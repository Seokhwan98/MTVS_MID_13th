using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class NPCController : MonoBehaviour
{
    public Transform[] waypoints;
    private int currentIndex = 0;
    private NavMeshAgent agent;
    
    public float wanderRadius = 10f;
    public float waitTime = 3f;
    private float timer = 0f;

    private bool isCalled = false;
    private Vector3 originalPosition;
    private Vector3 lastDisappearPosition;  
    
    
    // 버튼 이미지 관련
    public Button callButton;
    public Sprite callSprite;
    public Sprite byeSprite;

    public Animator animator;
    
    public float MoveSpeed = 5f;
    public float height = 2f; //공중 높이설정치
    public float rotationsize = 45f;
    
    [SerializeField] private DynamicNavMesh navMeshScript;
    
    private Transform mainCamera;
    
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        mainCamera = Camera.main.transform;
        originalPosition = transform.position;
    
        agent.updateUpAxis = true;  // Y축 자동 처리
        agent.updateRotation = true;
        agent.speed = MoveSpeed;
        agent.areaMask = NavMesh.AllAreas;
        agent.radius = 0.5f;  // 적당한 크기로 설정
        agent.height = 2f;    // 적당한 높이로 설정
        agent.baseOffset = height;  // Y축 높이를 height로 설정
        
        agent = GetComponent<NavMeshAgent>();
        agent.speed = 5f;
        MoveToRandomPoint(); // 처음 위치도 랜덤하게 이동 시작
    
        GameObject npcMap = GameObject.Find("NPCmap");  // npc 맵 오브젝트 이름을 정확하게 사용
        if (npcMap != null)
        {
            if (navMeshScript != null)
            {
                // NavMesh 빌드 완료 후 이동
                navMeshScript.OnNavMeshBuilt += MoveToNextPoint;
            }
            else
            {
                Debug.LogError("DynamicNavMesh 컴포넌트를 npc 맵에서 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogError("npc 맵 오브젝트를 찾을 수 없습니다.");
        }
        if (waypoints.Length > 0)
        {
            // 프리팹을 강제로 NavMesh 위로
            agent.Warp(waypoints[0].position);
            MoveToNextPoint();
        }
        else
        {
            Debug.LogWarning("waypoints가 비어 있습니다. 이동할 수 없습니다.");
        }
    }

    private void Update()
    {
        if (isCalled) return;
        
        if (navMeshScript != null && navMeshScript.IsNavMeshBuilt())
        {
            // NavMeshAgent의 위치는 자동으로 업데이트 되므로 transform.position을 수정하지 않습니다
            // Y축 고정은 NavMeshAgent에서 처리하도록 설정
            agent.updateUpAxis = true;  
            agent.baseOffset = height;  // 높이 적용 (baseOffset)

            // 이동 방향에 따른 회전
           
            timer += Time.deltaTime;

            if (!agent.pathPending && agent.remainingDistance < 0.5f && timer >= waitTime)
            {
                MoveToRandomPoint();
                timer = 0f;
            }
            Vector3 direction = agent.velocity;
            if (direction.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * rotationsize);
            }
        }

        // 경로 상태 확인
        if (agent.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            Debug.LogError("Path is invalid! Make sure that the waypoints are on the NavMesh.");
        }
        else if (agent.pathStatus == NavMeshPathStatus.PathPartial)
        {
            Debug.LogWarning("Path is partial! The agent might not be able to reach the destination.");
        }
        else if (agent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            Debug.Log("Path is complete!");
        }
        if (agent.hasPath)
        {
            Debug.Log("목표지점까지 가는 중...");
        }
        else
        {
            Debug.Log("경로 없음! 재시도");
            MoveToNextPoint();
        }
    }

    private void MoveToNextPoint()
    {
        if (waypoints.Length == 0)
        {
            Debug.Log("나임");
            
            return;
        }
        
        agent.SetDestination(waypoints[currentIndex].position);
        currentIndex = (currentIndex + 1) % waypoints.Length;
    }
    private void MoveToRandomPoint()
    {
        Vector3 randomPoint = GetRandomNavMeshLocation(wanderRadius);
        agent.SetDestination(randomPoint);
    }

    private Vector3 GetRandomNavMeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, radius, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return transform.position;
    }
    public void OnCallOrByeButtonClicked()
    {
        if (!isCalled)
        {
            CallToPlayer();
        }
        else
        {
            SendBackToWander();
        }
    }
    
    public void CallToPlayer()
    {
        isCalled = true;
        agent.isStopped = true; // 돌아다니기 중단
        
        // if (callButton != null) callButton.image.sprite = byeSprite; // 👈 버튼 이미지 바꾸기
        
        animator.SetTrigger("Appear");
        // gameObject.SetActive(false); // 잠깐 사라지기
        lastDisappearPosition = agent.destination;

        Invoke(nameof(TeleportAndAppearInFront), 1f); // 잠깐 딜레이 후 등장
    }
    
    private void SendBackToWander()
    {
        isCalled = false;

        // 현재 위치 저장
        
        // 사라지는 애니메이션 실행
        animator.SetTrigger("Disappear");

        // 버튼 이미지 변경
        if (callButton != null) callButton.image.sprite = callSprite;

        // 1초 후 다시 등장
        Invoke(nameof(GoBackAndAppear), 1f);
    }

    private void GoBackAndAppear()
    {
        // 사라졌던 위치로 이동
        agent.Warp(lastDisappearPosition);

        agent.isStopped = true;

        // 나타나는 애니메이션 실행
        animator.SetTrigger("Back");

        // 1.5초 후 다시 이동 시작
        Invoke(nameof(StartWandering), 1.5f);
    }

    private void TeleportAndAppearInFront()
    {
        // 카메라 앞 2미터 지점 계산
        Vector3 frontPosition = mainCamera.position + mainCamera.forward * 2f;
        frontPosition.y = mainCamera.position.y; // 높이 고정

        // NavMeshAgent를 통해 강제로 위치 이동
        agent.Warp(frontPosition);

        // 카메라 방향 보기
        transform.LookAt(mainCamera);

        // 애니메이션 실행
        animator.SetTrigger("AppearInFront");
    }
    
    private void StartWandering()
    {
        isCalled = false;
        agent.isStopped = false;
        MoveToRandomPoint();
    }
    
    
}
