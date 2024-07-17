using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class Target : MonoBehaviour
{
    [SerializeField] private GameObject[] WaypointGameObjects;
    [SerializeField] private float SpeedBullet = 10f;
    [SerializeField] public GameObject MyPlayer;
    [SerializeField] private GameObject PanelStart, PanelWin;
    
    private NavMeshAgent[] navAgents;

    private Animator _anim;
    private Vector3 _targetPos;
    private bool _isRun;
    private Vector3[] _wayPoints;
    private int _stepMove;
    private bool _lastWayNow;
    private bool _nowFire;
   
    Vector2 startPos;
    List<GameObject> _listEnemyInPoint;


    private void Awake()
    {
        _anim = MyPlayer.GetComponent<Animator>();

        navAgents = FindObjectsOfType(typeof(NavMeshAgent)) as NavMeshAgent[];

        _wayPoints = new Vector3[WaypointGameObjects.Length];
        for (int x = 0; x < WaypointGameObjects.Length; x++)
        {
            _wayPoints[x] = WaypointGameObjects[x].transform.GetChild(0).transform.position;
        }
    }

    private void UpdateTargets ( Vector3 targetPosition  )
    {
	    foreach(NavMeshAgent agent in navAgents) 
        {
            agent.isStopped = false;
            agent.destination = targetPosition;
	    }
    }

    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SetEnemy(GameObject g)
    {
        Enemy enemy =  g.GetComponent<Enemy>();

        if (enemy.SetDamage(1) <= 0)
        {
            g.gameObject.GetComponent<BoxCollider>().enabled = false;
            g.gameObject.GetComponent<Animator>().enabled = false;

            _listEnemyInPoint.Remove(g);

            if (_listEnemyInPoint.Count == 0)
            {
                _nowFire = false;

                if (_lastWayNow)
                {
                    PanelWin.SetActive(true);
                    Invoke(nameof(Restart), 3);
                }
                else
                {
                    _isRun = true;
                    _targetPos = _wayPoints[_stepMove];

                    UpdateTargets(_targetPos);
                }
            }
        }
        
    }

    void SmoothLookAt(Vector3 newDirection)
    {
        Vector3 lookDirection = newDirection - MyPlayer.transform.position;
        lookDirection.Normalize();

        MyPlayer.transform.rotation = Quaternion.Lerp(MyPlayer.transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime* 2f);
    }


    void Update()
    {
        if (_nowFire)
        {
            _anim.SetTrigger("Idle");
            if (_listEnemyInPoint.Count > 0) SmoothLookAt(_listEnemyInPoint[0].transform.position);
        }
       
        if (_isRun)
        {
            _anim.SetTrigger("Run");

            if (Vector3.Distance(MyPlayer.transform.position, _targetPos) < 0.2f)
            {
                _isRun = false;

                _listEnemyInPoint = new List<GameObject>();
                for (int x = 0; x < WaypointGameObjects[_stepMove].transform.childCount; x++)
                {
                    if (WaypointGameObjects[_stepMove].transform.GetChild(x).transform.tag == "Finish")
                        _listEnemyInPoint.Add(WaypointGameObjects[_stepMove].transform.GetChild(x).gameObject);
                }

                _nowFire = true;
                _stepMove++;

                if (_stepMove >= _wayPoints.Length)
                {
                    _lastWayNow = true;
                }
                foreach (NavMeshAgent agent in navAgents)
                {
                    agent.isStopped = true;
                }
           
            }
        }

   
        if (Input.touchCount > 0)
        {
            if (Input.touches[0].phase == TouchPhase.Began)
            {
                startPos = Input.touches[0].position; 
            
                if (_nowFire)
                {
                    Vector3 targetPosition = Vector3.zero;
                
                    Ray ray = Camera.main.ScreenPointToRay(startPos);
                    RaycastHit hitInfo;

                    if (Physics.Raycast(ray.origin, ray.direction, out hitInfo))
                    {
                        targetPosition = hitInfo.point;

                    }

                    GameObject bullet = ObjectPool.SharedInstance.GetPooledObject();
                    if (bullet != null)
                    {
                        bullet.transform.position = ObjectPool.SharedInstance.SpawnBullet.position;
                        bullet.transform.rotation = ObjectPool.SharedInstance.SpawnBullet.rotation;
                        bullet.SetActive(true);

                        bullet.GetComponent<Bullet>().Fire(targetPosition, SpeedBullet);
                    }
                }
                else
                {

                    if (_lastWayNow) return;

                    if (PanelStart.activeSelf) PanelStart.SetActive(false);

                   
                    _isRun = true;
                    _targetPos = _wayPoints[_stepMove];

                    UpdateTargets(_targetPos);
                }


            }

            
        }
              
        
    }

    private bool GetInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            return true;
        }
        return false;
    }
}