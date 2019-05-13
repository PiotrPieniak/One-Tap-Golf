using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private const int parabolaElementsAmount = 10; // Number of elements should draw in path of parabola
    private const int parabolaBaseHeight = 1;
    private const float parabolaHeightOffset = 0.015f;
    private const float parabolaHeightMultiplier = 0.003f;
    private const float parabolaXRangeOffset = 0.05f;
    private const float parabolaXRangeMultiplier = 0.005f;
    private const float ballParaboleStartPos = 0.05f;
    private const float ballMovementEdge = -3.5f;
    private const float ballMovementSpeed = 0.014f;
    private const float viewfinderMaxTransformPosX = 8.55f;
    
    private Vector2 viewfinderBaseTransformPos;
    private Vector2 ballStartTransformPos;
    
    public GameObject MoveableObjRef; // Objet to move on path
    public GameObject TrajectoryElementObjRef; // GO to draw in path of trajectory.
    public GameObject ViewfinderObjRef;
    public LoseMenu LoseMenu;
    public Transform ParabolaStart; // Start position from where trajectory should start and mouse of finger position is the destination
    public Transform ParabolaEnd;
    private GameObject HoleObjRef;
    private Hole HoleScriptRef;
    
    private float parabolaHeight;
    private float viewfinderTransformPosX;
    private float ballParabolePos; //start point of ball on parabole
    
    private bool ballCanGo;
    private bool isBallInAir;
    private bool ballSpawnBlocked;
    private bool actionTaken;
    
    private int score;
    private int scoreBeforeShot;
    private int scoreAfterShot;

    Vector3 a, b; //Vector positions for start and end
    List<GameObject> trajectoryElementsContainer = new List<GameObject> ();

    void Start ()
    {
        HoleObjRef = GameObject.FindGameObjectWithTag("Hole");
        HoleScriptRef = HoleObjRef.GetComponent<Hole>();
        parabolaHeight = parabolaBaseHeight;
        ballParabolePos = ballParaboleStartPos;
        ballStartTransformPos = new Vector2(MoveableObjRef.transform.position.x, MoveableObjRef.transform.position.y);
        viewfinderBaseTransformPos = new Vector2(ViewfinderObjRef.transform.position.x, ViewfinderObjRef.transform.position.y);
        
        viewfinderTransformPosX = ViewfinderObjRef.transform.position.x;

        
        for (int i = 0; i < parabolaElementsAmount; i++)
            trajectoryElementsContainer.Add(Instantiate(TrajectoryElementObjRef));
    }

    void Update ()
    {
        score = HoleScriptRef.score;
        if (Input.GetMouseButton(0) && !isBallInAir)
        {
            if (viewfinderTransformPosX <= viewfinderMaxTransformPosX)
            {
                parabolaHeight += parabolaHeightOffset + (parabolaHeightMultiplier * score);
                viewfinderTransformPosX += parabolaXRangeOffset + (parabolaXRangeMultiplier * score);
                ViewfinderObjRef.transform.position = new Vector2(viewfinderTransformPosX, viewfinderBaseTransformPos.y);
                
                if (ParabolaStart)
                {
                    a = ParabolaStart.position; //Get vectors from the transforms
                    a = new Vector3 (a.x, a.y, 0);
                    b = ParabolaEnd.position;
                    b = new Vector3 (b.x, b.y, 0);
            
                    foreach (GameObject Dot in trajectoryElementsContainer)
                    {
                        Dot.SetActive(true);
                    }
                    
                    ViewfinderObjRef.SetActive(true);

                    for (float i = 1; i <= parabolaElementsAmount; i++) {
                        Vector3 currentPosition = SampleParabola(a, b, parabolaHeight, i / parabolaElementsAmount);
                        trajectoryElementsContainer [(int)i - 1].transform.position = new Vector3 (currentPosition.x, currentPosition.y, 0);

                        Vector3 nextPosition = SampleParabola(a, b, parabolaHeight, (i + 1) / parabolaElementsAmount);
                        float angleInR = Mathf.Atan2 (nextPosition.y - currentPosition.y, nextPosition.x - currentPosition.x);
                        trajectoryElementsContainer [(int)i - 1].transform.eulerAngles = new Vector3 (0, 0, Mathf.Rad2Deg * angleInR - 90);
                    }
                    ballCanGo = true;
                }
            }
            else 
            {
                scoreBeforeShot = score;
                isBallInAir = true;
            }
        }

        if (Input.GetMouseButton(0) == false && ballCanGo && actionTaken == false)
        {
            scoreBeforeShot = score;
            actionTaken = true;
            isBallInAir = true;
        }

        if (isBallInAir)
        {
            foreach (GameObject Dot in trajectoryElementsContainer)
            {
                Dot.SetActive(false);
            }
            
            ViewfinderObjRef.SetActive(false);
            MoveableObjRef.transform.position = SampleParabola(a, b, parabolaHeight, ballParabolePos);
            ballParabolePos += ballMovementSpeed;

            if (MoveableObjRef.transform.position.y < ballMovementEdge)
            {
                if (actionTaken)
                {
                    isBallInAir = false;
                    ballCanGo = false;
                    actionTaken = false;
                    ViewfinderObjRef.transform.position = new Vector2(viewfinderBaseTransformPos.x, viewfinderBaseTransformPos.y);
                    viewfinderTransformPosX = viewfinderBaseTransformPos.x;
                    parabolaHeight = parabolaBaseHeight;
                    ballParabolePos = ballParaboleStartPos;
                    gameObject.SetActive(false);
                    Invoke("BallFallReaction", 1);
                }
            }
        }
    }

    void BallFallReaction()
    {
        scoreAfterShot = score;
                
        if ((scoreAfterShot != scoreBeforeShot) && !ballSpawnBlocked)
        {      
            Invoke("RestartBallPosition", 1.2f);
        }
                
        if (scoreAfterShot == scoreBeforeShot)
        {
            Invoke("ShowLoseMenu", 1);
        }
    }

    void RestartBallPosition()
    {
        transform.position = new Vector2(ballStartTransformPos.x, ballStartTransformPos.y);
        gameObject.SetActive(true);
    }

    void ShowLoseMenu()
    {
        ballSpawnBlocked = true;
        HoleScriptRef.score = 0;

        if (PlayerPrefs.GetFloat("Highscore") < scoreAfterShot)
        {
            PlayerPrefs.SetFloat ("Highscore", scoreAfterShot);
        }
                    
        LoseMenu.ToggleLoseMenu (scoreAfterShot);
    }

    void OnDrawGizmos ()
    {
        //Draw the parabola by sample a few times
        Gizmos.color = Color.red;
        Gizmos.DrawLine (a, b);
        float count = 20;
        Vector3 lastP = a;
        for (float i = 0; i < count + 1; i++) {
            Vector3 p = SampleParabola (a, b, parabolaHeight, i / count);
            Gizmos.color = i % 2 == 0 ? Color.blue : Color.green;
            Gizmos.DrawLine (lastP, p);
            lastP = p;
        }
    }

    /// <summary>
    /// Get position from a parabola defined by start and end, height, and time
    /// </summary>
    /// <param name='start'>
    /// The start point of the parabola
    /// </param>
    /// <param name='end'>
    /// The end point of the parabola
    /// </param>
    /// <param name='height'>
    /// The height of the parabola at its maximum
    /// </param>
    /// <param name='t'>
    /// Normalized time (0->1)
    /// </param>S
    Vector3 SampleParabola (Vector3 start, Vector3 end, float height, float t)
    {
        float parabolicT = t * 2 - 1;
        if (Mathf.Abs (start.y - end.y) < 0.1f) {
            //start and end are roughly level, pretend they are - simpler solution with less steps
            Vector3 travelDirection = end - start;
            Vector3 result = start + t * travelDirection;
            result.y += (-parabolicT * parabolicT + 1) * height;
            return result;
        } else {
            //start and end are not level, gets more complicated
            Vector3 travelDirection = end - start;
            Vector3 levelDirecteion = end - new Vector3 (start.x, end.y, start.z);
            Vector3 right = Vector3.Cross (travelDirection, levelDirecteion);
            Vector3 up = Vector3.Cross (right, travelDirection);
            if (end.y > start.y)
                up = -up;
            Vector3 result = start + t * travelDirection;
            result += ((-parabolicT * parabolicT + 1) * height) * up.normalized;
            return result;
        }
    }
}
