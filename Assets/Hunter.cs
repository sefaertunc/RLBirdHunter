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

    [Header("Rotation Variables")]
    public float rotationSpeed = 2f;  // D�n�� h�z�
    public float detectionDelay = 0.5f;  // Alg�lama gecikmesi
    private Transform target;  // Hedef obje
    private bool isTargetInView;  // Hedefin g�r�� alan�nda olup olmad���n� kontrol eder

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
                            if (!isTargetInView)
                            {
                                isTargetInView = true;
                                StartCoroutine(HandleTargetDetection());
                            }
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
        }
    }

    IEnumerator HandleTargetDetection()
    {
        yield return new WaitForSeconds(detectionDelay);
        while (isTargetInView)
        {
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

        Gizmos.color = Color.yellow;

        Vector3 startPosition = transform.position;
        Vector3 startDirection = Quaternion.Euler(0, -viewAngle / 2, 0) * transform.forward;
        Vector3 previousPoint = startPosition + startDirection * viewDistance;
        float angleStep = viewAngle / segments;

        for (int i = 1; i <= segments; i++)
        {
            float currentAngle = -viewAngle / 2 + angleStep * i;
            Vector3 nextDirection = Quaternion.Euler(0, currentAngle, 0) * transform.forward;
            Vector3 nextPoint = startPosition + nextDirection * viewDistance;

            Gizmos.DrawLine(startPosition, previousPoint);
            Gizmos.DrawLine(previousPoint, nextPoint);

            previousPoint = nextPoint;
        }

        Gizmos.DrawLine(startPosition, previousPoint);
    }
}
