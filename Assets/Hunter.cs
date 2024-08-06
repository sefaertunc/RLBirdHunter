using System.Collections;
using UnityEngine;

public class Hunter : MonoBehaviour
{
    [Header("Roaming Variables")]
    public float startAngle; // Ba�lang�� a��s�
    public float endAngle; // Biti� a��s�
    public float roamingRotationSpeed; // D�nme h�z�
    private bool isReversing = false; // D�nme y�n�n� kontrol eden de�i�ken

    [Header("FOV Variables")]
    public float viewAngle = 60f;  // G�r�� a��s�
    public float viewDistance = 10f;  // G�r�� mesafesi
    public int segments = 10;  // FOV segmentleri
    public LayerMask targetMask;  // Hedef katman�
    public string targetTag = "bird";  // Hedef tag
    public float gizmoAlpha = 0.5f; // Gizmos transparanl�k de�eri

    [Header("Rotation Variables")]
    public float rotationSpeed = 2f;  // D�n�� h�z�
    public float detectionDelay = 0.5f;  // Alg�lama gecikmesi
    private Transform target;  // Hedef obje
    private bool isTargetInView;  // Hedefin g�r�� alan�nda olup olmad���n� kontrol eder
    private bool hasTarget = false;  // Mevcut hedefin olup olmad���n� kontrol eder

    void Update()
    {
        if (!isTargetInView)
        {
            Roaming();
        }
        DetectTargets();

        // Hedef alandan ��k�ld���nda yatay d�zleme geri d�n
        if (!isTargetInView && target == null)
        {
            ResetRotation();
        }
    }

    void Roaming()
    {
        float currentAngle = transform.eulerAngles.y;
        float targetAngle = isReversing ? startAngle : endAngle;

        // A��lar aras�ndaki fark� hesapla
        float angleDifference = Mathf.DeltaAngle(currentAngle, targetAngle);

        // Hedef a��y� ge�medi�i s�rece d�n�� yap
        if (Mathf.Abs(angleDifference) > 0.1f)
        {
            float step = roamingRotationSpeed * Time.deltaTime;
            currentAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, step);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, currentAngle, transform.eulerAngles.z);
        }
        else
        {
            // Hedef a��y� ge�ince y�n de�i�tir
            isReversing = !isReversing;
        }
    }

    void DetectTargets()
    {
        if (hasTarget) return; // Hedefimiz varken yeni hedef aramay�z

        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewDistance, targetMask);

        bool previousTargetInView = isTargetInView;
        isTargetInView = false; // Varsay�lan olarak hedef g�r�� alan�nda de�il

        foreach (Collider potentialTarget in targetsInViewRadius)
        {
            if (potentialTarget.CompareTag(targetTag))
            {
                Vector3 dirToTarget = (potentialTarget.transform.position - transform.position).normalized;
                float angleToTarget = Vector3.Angle(transform.forward, dirToTarget);

                if (angleToTarget < viewAngle / 2)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, dirToTarget, out hit, viewDistance))
                    {
                        Debug.DrawLine(transform.position, hit.point, Color.red);

                        if (hit.transform.CompareTag(targetTag))
                        {
                            target = potentialTarget.transform;
                            hasTarget = true;
                            if (!isTargetInView)
                            {
                                isTargetInView = true;
                                StartCoroutine(HandleTargetDetection());
                            }
                            break; // �lk hedefi buldu�umuzda di�erlerini aramay� b�rak
                        }
                    }
                    else
                    {
                        Debug.DrawLine(transform.position, transform.position + dirToTarget * viewDistance, Color.red);
                    }
                }
            }
        }

        if (!isTargetInView)
        {
            target = null;
            hasTarget = false;
        }
    }

    IEnumerator HandleTargetDetection()
    {
        yield return new WaitForSeconds(detectionDelay);
        while (isTargetInView)
        {
            if (target == null || Vector3.Distance(transform.position, target.position) > viewDistance || Vector3.Angle(transform.forward, (target.position - transform.position).normalized) > viewAngle / 2)
            {
                isTargetInView = false; // Hedef alan d���na ��kt� veya kaybedildi
                target = null;
                hasTarget = false;
                yield break;
            }

            // Hedefin pozisyonuna do�ru raycast �iz
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, dirToTarget, out hit, viewDistance))
            {
                Debug.DrawLine(transform.position, hit.point, Color.red);
            }
            else
            {
                Debug.DrawLine(transform.position, transform.position + dirToTarget * viewDistance, Color.red);
            }

            RotateTowardsTarget();
            yield return null;
        }
    }

    void RotateTowardsTarget()
    {
        if (target == null)
            return;

        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction); // Hedefe tam bak��
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    void ResetRotation()
    {
        // Karakterin sadece yatay d�zlemde s�f�rlanmas�
        Quaternion currentRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0, currentRotation.eulerAngles.y, 0);
        transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    private void OnDrawGizmos()
    {
        if (isTargetInView)
            return; // Hedef g�r�� alan�ndayken gizmoslar� �izme

        Color gizmoColor = new Color(0.7f, 1f, 0.7f, gizmoAlpha); // Sar� renk ve transparanl�k ayar�
        Gizmos.color = gizmoColor;

        Vector3 startPosition = transform.position;

        // Draw the FOV as a 3D cone
        for (int i = 0; i <= segments; i++)
        {
            float angle = -viewAngle / 2 + (viewAngle / segments) * i;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward * viewDistance;
            Gizmos.DrawLine(startPosition, startPosition + direction);

            for (int j = 0; j <= segments; j++)
            {
                float verticalAngle = -viewAngle / 2 + (viewAngle / segments) * j;
                Vector3 verticalDirection = Quaternion.Euler(verticalAngle, 0, 0) * direction;
                Gizmos.DrawLine(startPosition, startPosition + verticalDirection);
            }
        }
    }
}
