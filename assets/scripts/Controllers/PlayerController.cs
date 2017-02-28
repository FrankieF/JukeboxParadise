using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using XboxCtrlrInput;
using XInputDotNetPure;

public class PlayerController : MonoBehaviour
{
    #region Class Variables

    [Header("NEVER CHANGE")]
    public bool neverChange;
    public bool inGame;
    public int playerNumber;
    public int reset = 0;
    public int hitMax = 3; // The number of hits to drop an object
    public int teamNumber;
    public string color; // References colors for trys and win screen
    public string charcterSelectAudio;
    public string characterName;
    public GameObject pNumber;
    public UILabel p;

    // These are used for setting the vibrations of the controller    
    private float countDown;
    private bool vibrate; // This makes the controller vibrate
    private int index;
    private PlayerIndex[] PlayerIndexLookup;

    [Header("Movement")]    
    public bool headerMovement;
    public bool shouldRotate = true;
    public bool spinning; // Allows the player to counter
    public int spinCounter = 0, spinMax = 5;
    public float speed;
    private float maxSpeed = 10f;
    public float rotatingDazed = 1f;
    public float gravity = 50f;
    public float spinCheck = 0f;
    private float maxVelocityChange = 10f;
    private Vector3 dodgeDirection;
    private Vector3 targetVelocity;
    
    // Used to detect player movement and apply skid if player changes direction too fast
    public float maxAnalogChange = 1f;
    private float delta;
    private Vector2 currentInput;
    private Vector2 previousInput;   

    [Header("Attack")]
    public bool headerForce = true;
    public bool contact;
    public float force = 0f;  
    public float critTimer;
    public float criticalCheck = 0f;
    public float punchTime;
    public float increasedPunchTime;
    public float weakAttack = 15f;
    public float mediumAttack = 22f;
    public float strongAttack = 30f;
    public int hitCounter = 0; // Count of times hit while holding object
    public string punchType = "", strongPunch = "SFX_strongPunch", mediumPunch = "SFX_medPunch", weakPunch = "SFX_weakPunch";
    public GameObject attackTrigger;

    [Header("Particles")]
    public bool headerParticle;
    public ParticleSystem startRunParticle = new ParticleSystem(), skiddingParticle = new ParticleSystem(),
                          punchParticle = new ParticleSystem(), dazePunchParticle = new ParticleSystem(),
                          ingredientPunchParticle = new ParticleSystem(), spinParticle = new ParticleSystem(),
                          friendPunchParticle = new ParticleSystem(), spinHitParticle = new ParticleSystem();
    public GameObject pivot, dazedObject; // The point the trail renders spin around
    public GameObject[] particlesToChange;

    [Header("Pick up and Drop")]
    public bool headerThrow;
    public bool canPick = true;
    public bool hasObject; // Checks to see if player has an object
    public bool hasScored;
    public string pickReference = "PickReference";
    public GameObject pickObject;
    public GameObject pickRef;
    public GameObject rangeTrigger; // The triggers are used to detect items to pick up
    private GameObject tempObject; // The player is stored inside of this to attack them
    public Vector3 dropDirection = Vector3.up; // Direction player drops the object
    public Tray tray; // To reference the target particles

    // These are references to other classes that are used inside of this class
    CameraController cameraController;

    [Header("Stats")]
    public bool headerStats;
    public int punches, hits, dazes, counters, scored, knockedOff, johnnyHit, sophiaHit, gingerHit, louisHit; // The different categories for the stat screen


    #endregion
    
    #region PlayerStates

    [Header("Player States")]
    public bool headerState;

    public Animator animator;

    // List of possible player states
    public enum PlayerStates
    {
        None,
        Idle,
        Run,
        Ingredient,
        Stopping,
        Trip,
        KnockBack,
        Spin,
        Punch,
        PickUp,
        Place,
        Dead,
        Respawn,
        Hit,
        CriticalHit,
        Dance,
        
        Max
    }
    
    // Keeps track of the player states and how to change them
    public PlayerStates currentState = PlayerStates.None;
    public PlayerStates previousState = PlayerStates.Idle;
    public PlayerStates targetState = PlayerStates.Idle;
    public PlayerStates pastState = PlayerStates.None;

    [Header("Audio")]
    public bool headerAudio;
    // Audio
    public AudioClip[] sounds;
    protected float voTimer; 

    [Header("Player References")]
    public bool headerPlayerRefl;
    public PlayerRef pRef; // Used to access stored variables from Main Menu
    public GameObject ring; // Character ring to change the color for teams
    public Color blueColor = new Color(0f, .95f, .5f), pinkColor = new Color(1f, 0f, .5f); // Colors to change the rings

    #endregion
    
    #region Awake/Start Functions
    
    // Use this for initialization
    public virtual void Start()
    {        
        PlayerIndexLookup = new PlayerIndex[5];        
        GetPlayerIndex();
        pickRef = GameObject.Find("PickRef");
    }
    
    void GetPlayerIndex()
    {
        PlayerIndexLookup [0] = PlayerIndex.One;
        PlayerIndexLookup [1] = PlayerIndex.One;
        PlayerIndexLookup [2] = PlayerIndex.Two;
        PlayerIndexLookup [3] = PlayerIndex.Three;
        PlayerIndexLookup [4] = PlayerIndex.Four;  
    }
    
    #endregion

    public virtual void Update()
    {
        // Gets the anolog input from the player
        // Checks to see if the X & Y are in different directions then the previous frames
        // Also checks the distance the analog stick has traveled
        // If the analog stick has traveled too far and the directions are different the player will skid
        previousInput = currentInput;
        targetVelocity = new Vector3(-XCI.GetAxis(XboxAxis.LeftStickX, playerNumber), 0f, -XCI.GetAxis(XboxAxis.LeftStickY, playerNumber));
        targetVelocity.Normalize();
        currentInput = new Vector2(targetVelocity.x, targetVelocity.z);
        
        Vector2 prevDir = new Vector2(previousInput.x > 0 ? 1 : previousInput.x < 0 ? -1 : 0, previousInput.y > 0 ? 1 : previousInput.y < 0 ? -1 : 0);
        Vector2 currentDir = new Vector2(currentInput.x > 0 ? 1 : currentInput.x < 0 ? -1 : 0, currentInput.y > 0 ? 1 : currentInput.y < 0 ? -1 : 0);
        
        delta = Vector2.Distance(currentInput, previousInput);
        
        if (currentState == PlayerStates.Run && delta > maxAnalogChange && (prevDir.x != currentDir.x || prevDir.y != currentDir.y))
        {
            if (speed >= maxSpeed)
            {
                SetState(PlayerStates.Stopping);
            }
        }

        // Controls player states
        if (currentState != targetState)
        {
            StopCoroutine(currentState.ToString());
            pastState = previousState;
            previousState = currentState;
            currentState = targetState;
            StartCoroutine(currentState.ToString());
        }
        
        Counters();
    }
    
    #region Movement

    // Everything with the players movement is inside fixed update
    void FixedUpdate()
    {
        rigidbody.drag = 4f;               

        if (targetVelocity.magnitude > 1f)
            targetVelocity.Normalize();

        targetVelocity *= speed;          
        
        // Applies force to reach target velocity
        Vector3 velocity = rigidbody.velocity;

        // If the state is idle the player will skid to a stop
        // Else the player will have normal acceleration
        if (!ApplySlowDown())
        {            
            Vector3 lookDirection = targetVelocity.normalized;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * 10f); // Applies rotation

            Vector3 velocityChange = (targetVelocity - velocity);
            
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;
            
            rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
        } 
        else
            TypeOfSlowDown(targetState);
        
        // Used to apply gravity
        rigidbody.AddForce(new Vector3(0f, -gravity * rigidbody.mass, 0f));        
    }

    bool ApplyRotation() // If the player is in one of the states it will not update rotation
    {
        return currentState.Equals(PlayerStates.Spin) ? true : false;
    }

    bool ApplySlowDown() // If the player has an ingredient or is running it will update velocity
    {
        return targetState.Equals(PlayerStates.Run) || targetState.Equals(PlayerStates.Ingredient) ? false : true;
    }

    void TypeOfSlowDown(PlayerStates state)
    {
        if (state.Equals(PlayerStates.Punch) || state.Equals(PlayerStates.Spin))
        {
            rigidbody.drag = 0f;
            return;
        } 
    }

    void OnCollisionEnter(Collision collision)
    {
        int world = 8;
        if (collision.gameObject.layer == world)
        {
            Vibrate(playerNumber, .75f, .75f, .15f);
        }
    }
    
    #endregion
    
    /* Everything regarding the player is inside this function
     * The attack, dash and tackle functions are called here
     * This function is then called in update and will check for the player actions
     */ 
    
    // Keeps track of all the timers
    void Counters()
    {
        if (hitCounter >= hitMax)
        {
            SetState(PlayerStates.CriticalHit);
            hitCounter = reset;
        }

        if(criticalCheck <= reset)
        {
            if (hitCounter > 0)
                hitCounter--;

            criticalCheck = 3f;
        }

        if (countDown <= reset)
        {
            index = playerNumber;
            GamePad.SetVibration(PlayerIndexLookup [index], 0.0f, 0.0f);
            vibrate = false;
        }

        criticalCheck -= Time.deltaTime;
        countDown -= Time.deltaTime;
    }
    
    //makes the controller vibrate 
    void Vibrate(int number, float left,
                 float right, float time)
    {
        if (!vibrate)
        {
            index = number;
            
            GamePad.SetVibration(PlayerIndexLookup [index], left, right);
            
            countDown = time;
            
            vibrate = true;
        }
    }

    
    #region State Machine
    
    // Changes the state for the player
    public void SetState(PlayerStates state)
    {
        targetState = state;
    }

    // Players can be set to none to be disabled
    IEnumerator None()
    {
        animator.SetBool("running", false);
        animator.SetBool("moving", false);
        animator.SetBool("ingredient", false);
        animator.SetBool("returnToIdle", true);



        while (true)
        {            
            yield return null;
        }
    }
    
    #region Moving

    // Idle is used when the player is not moving
    IEnumerator Idle()
    {
        if (hasObject)
            animator.SetBool("ingredient", true);

        animator.SetBool("spinning", false);
        animator.SetBool("moving", false);
        animator.SetBool("skidding", false);
        animator.SetBool("running", false);
        animator.SetBool("pickUp", false);
        animator.SetBool("returnToIdle", false);
        animator.SetBool("spinDaze", false);
        startRunParticle.Stop();

        ResetSpinParticle();

        float minSpeed = 1f;
        speed = minSpeed;
        contact = false;

        while (true)
        {
            if(shouldRotate)
                transform.RotateAround(Vector3.zero, Vector3.up, .5f);

            if (TryPunch())
            {               
                yield break;
            }
            
            if (XCI.GetAxis(XboxAxis.LeftStickX, playerNumber) != 0 || XCI.GetAxis(XboxAxis.LeftStickY, playerNumber) != 0)
            {
                if (!hasObject)
                    SetState(PlayerStates.Run);
                else                
                    SetState(PlayerStates.Ingredient);
            }
            
            SetSpin();
            TryThrowOrPickup();
            
            yield return null;
        }
    }

    IEnumerator Run()
    {
        animator.SetBool("moving", true); // start run animation

        bool set = false; // Checks to increase speed
        bool speedSet = false;
        float runSpeed = 7f, speedIncrease = .5f, accelSpeed = 18f; // The speed the player starts running, accelerates and maxs out aat

        speed = runSpeed;

        while (true)
        {
            if (speed < accelSpeed && !speedSet) // If speed is not at max increase it and play particle
            {
                speed += speedIncrease;
                startRunParticle.Play();
                if(speed >= accelSpeed)
                    speedSet = true;
            }
            else if (speed >= maxSpeed && !set)
            {
                speed = maxSpeed;
                animator.SetBool("running", true); // Run animation
                set = true; // Stop accelerating
                startRunParticle.Stop();
            }

            SetSpin(); // Checks for all possible actions
            TryPunch();
            ReturnToIdle();
            TryThrowOrPickup();
            
            yield return null;
        }
    }
    
    IEnumerator Ingredient()
    {
        float ingredientSpeed = 9f;

        speed = ingredientSpeed;
        animator.SetBool("moving", true);
        
        while (true)
        {
            TryThrowOrPickup();
            ReturnToIdle();
            SetSpin();
            yield return null;
        }
    }

    IEnumerator Stopping()
    {
        float stop = .12f;
        animator.SetBool("moving", false);
        animator.SetBool("running", false);
        animator.SetBool("skidding", true);
        skiddingParticle.Play();

        while (true)
        {
            yield return new WaitForSeconds(stop);
            SetState(PlayerStates.Idle);
        }
    }
    
    #endregion 
    
    #region Actions
    
    IEnumerator Spin()
    {
        if (pastState == PlayerStates.Spin)
            spinCounter++;
        else
            spinCounter = reset;

        animator.SetBool("spinning", true);
        float spin = .38f, spinTimer = .1f;

        spinning = true; // Stops the player from being hit

        //Plays SPIN SFX
        AudioController.Play ("SFX_Spin");

        spinParticle.Clear(true);
        spinParticle.Play(); // Clears the particles from the last spin and plays the spin particles
        foreach (var p in spinParticle.GetComponentsInChildren<ParticleSystem>())
        {
            p.simulationSpace = ParticleSystemSimulationSpace.World;
            p.Play();
        }

        spinParticle.GetComponentInChildren<Animator>().SetTrigger("playAnim");
        spinning = true;
        while (true)
        {
            if(shouldRotate)
                transform.RotateAround(Vector3.zero, Vector3.up, .5f);

            spin -= Time.deltaTime;

            yield return null;
   
            if(spin <= spinTimer)
                spinning = false;
            if(spin <= reset)
            {
//                animator.SetBool("moving", false);
//                animator.SetBool("running", false);

                if(spinCounter >= spinMax) // Checks if the player has spun too much
                {
                    animator.SetBool("daze", true);
                    animator.SetBool("returnToIdle", true);
                    animator.SetBool("spinning", false);
                    SetState(PlayerStates.CriticalHit);
                }
                else
                {
                    animator.SetBool("spinning", false);
                    SetState(PlayerStates.Idle); 
                }
            }
        }
    }
    
    IEnumerator Punch()
    {
        punchTime = .2f;
        animator.SetBool("punching", true);

        punches++; // Increases the number of times the player punched

        while (true)
        {
            if(shouldRotate)
                transform.RotateAround(Vector3.zero, Vector3.up, .5f);

            punchTime -= Time.deltaTime;
            yield return null;

            if(punchTime <= reset)
            {
                animator.SetBool("moving", false);
                animator.SetBool("running", false);
                animator.SetBool("punching", false);
                SetState(PlayerStates.Idle);
            }
        }
    }

    IEnumerator PickUp()
    {
        float pickTime = .3f;

        while (true)
        {
            yield return new WaitForSeconds(pickTime);

            SetState(PlayerStates.Idle);
        }
    }
    
    IEnumerator Place()
    {
        float placeTime = .4f;

        while (true)
        {
            if(shouldRotate)
                transform.RotateAround(Vector3.zero, Vector3.up, .5f);

            yield return new WaitForSeconds(placeTime);

            animator.SetBool("throw", false);

            SetState(PlayerStates.Idle);
        }
    }
    
    IEnumerator Dead()
    {        
        float respawnTime = 1.5f;
        
        while (true)
        {
            yield return new WaitForSeconds(respawnTime);
            
            SetState(PlayerStates.Respawn);
        }
    }
    
    IEnumerator Respawn()
    {
        float spawnTime = .5f;
        
        //Plays when character is respawned on stage 
        respawnVO(); 
        
        transform.position = GetSpawnPoints.GetInstance.RespawnPoints [Random.Range(0, GetSpawnPoints.GetInstance.RespawnPoints.Count)].transform.position; 
        
        
        while (true)
        {
            yield return new WaitForSeconds(spawnTime);
            
            SetState(PlayerStates.Idle);
        }
    }
    
    IEnumerator Hit()
    {        
        animator.SetTrigger("hit");   
        animator.SetBool("punching", false);
        punchedVO(); 

        float hitTime = .5f;

        while (true)
        {
            yield return new WaitForSeconds(hitTime);
            
            SetState(PlayerStates.Idle);
        }
    }

    IEnumerator CriticalHit()
    {
        float criticalHitTime = 2f, t = 0f;
        dazedObject.SetActive(true);
        animator.SetBool("dazed", true);
        dazedVO (); 

        foreach (var p in dazedObject.GetComponentsInChildren<ParticleSystem>())
        {
            p.Play();
        }

        if (pickObject != pickRef)
            DropObject();

        while (true)
        {
            if(shouldRotate)
                transform.RotateAround(Vector3.zero, Vector3.up, .5f);
            t += Time.deltaTime;
            yield return null;
            if(t>=criticalHitTime)
            {
                animator.SetBool("dazed", false);
                dazedObject.SetActive(false);
                SetState(PlayerStates.Idle);
                respawnVO ();
            }
        }
    }

    IEnumerator Dance()
    {
        animator.SetBool("dance", true);
        
        while(true)
        {
            yield return null;
        }
    }
    
    #endregion

    #region State Functions

    // Calls the dodge state
    void SetSpin()
    {
        if (XCI.GetButtonDown(XboxButton.Y, playerNumber))
        {
            startRunParticle.Stop();
            SetState(PlayerStates.Spin);
        }
    }
    
    bool TryPunch()
    {
        if (!hasObject)
        {
            if (XCI.GetButtonDown(XboxButton.X, playerNumber))
            {   
                startRunParticle.Stop();
                SetState(PlayerStates.Punch);
                punchingVO(); 
                return true;
            }
        }
        return false;
    }

    void TryThrowOrPickup()
    {
        if (XCI.GetButtonDown(XboxButton.A, playerNumber))
        {
            startRunParticle.Stop();
            if(!hasObject)
            {
                SetState(PlayerStates.PickUp);
                animator.SetBool("pickUp", true);
            }
            else
            {
                SetState(PlayerStates.Place);
                animator.SetBool("throw", true);
            }
        }
    }

    void ResetSpinParticle()
    {
        pivot.transform.position = transform.position;
    }
    
    // Checks for the punch input
    void ReturnToIdle()
    {
        if (XCI.GetAxis(XboxAxis.LeftStickX, playerNumber) == 0 && XCI.GetAxis(XboxAxis.LeftStickY, playerNumber) == 0)
        {
            startRunParticle.Stop();
            animator.SetBool("moving", false);
            animator.SetBool("running", false);
            SetState(PlayerStates.Idle);
        }
    }
    
    // Checks if the player is moving and then decides which state to return too
    void ReturnFromState()
    {
        if (XCI.GetAxis(XboxAxis.LeftStickX, playerNumber) != 0 || XCI.GetAxis(XboxAxis.LeftStickY, playerNumber) != 0)
        {
            SetState(PlayerStates.Run);
            
        } else
            SetState(PlayerStates.Idle);
    }

    #endregion
    
    #endregion

    #region Reset States

    public void ResetStates()
    {
        animator.SetBool("moving", false); // Turns all of the possible animation parameters false so the players return to idle
        animator.SetBool("running", false);
        animator.SetBool("ingredient", false);
        animator.SetBool("punching", false);
        animator.SetBool("skidding", false);
        animator.SetBool("spinning", false);
        animator.SetBool("dazed", false);
        animator.SetBool("throw", false);
        animator.SetBool("pickUp", false);
        animator.SetBool("returnToIdle", false);
        hasObject = false; // The player no longer has an object
        pickObject = pickRef;
        rangeTrigger.GetComponent<Picking>().objs.Clear(); // Clears the list of items to pick up
        attackTrigger.GetComponent<Attacking>().colliders.Clear(); // Clears the list of objects to punch
        StopParticles();
    }

    void StopParticles()
    {
        punchParticle.Stop();
        dazedObject.SetActive(false);
        dazePunchParticle.Stop();
        friendPunchParticle.Stop();
        //spinParticle.Stop();
        ingredientPunchParticle.Stop();
    }

    #endregion

    #region Player Respawn
    
    public void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("DeathVolume"))
        {
            //Plays when character goes off stage 
            stageOffVO();
            
            if (hasObject)
                DropObject();
            
            StopCoroutine(currentState.ToString());
            SetState(PlayerStates.Dead);
        } 
    }
    
    #endregion


    #region Particles

    void PlayStartRun()
    {
        if (speed < maxSpeed)
            startRunParticle.Play();
        else
            startRunParticle.Stop();
    }

    #endregion
    
    #region Attack
    
    public void CheckAttack()
    {
        Attacking attacking = attackTrigger.GetComponent<Attacking>(); 

        if (attacking.colliders.Count > 0)
        {
            string playerTag = "Player";
            float objectDistance = 10f;

            foreach(var a in attacking.colliders) // Gets the closest distance of the colliders and tells the player to punch that object
            {
                float dis = Vector3.Distance(transform.position, a.transform.position);
                if(dis < objectDistance)
                {
                    objectDistance = dis;
                    tempObject = a;
                }
            }

            if(tempObject.CompareTag(playerTag)) // Checks to see if it is a player it is punching
            {
                if (CannotHit())
                {
                   
                    rigidbody.AddForce(-transform.forward, ForceMode.Impulse);
                    contact = true;
                    return;
                }

                PlayerController pc = tempObject.GetComponent<PlayerController>();

                // This is the direction to apply the force in
                Vector3 relativePosition = transform.forward;
                relativePosition.Normalize();
                CalculateAttackPower();
                
                if(HitSpin()) // If the player hits someone spining they get hit
                {
                    //Plays the counter hit 
                    AudioController.Play ("FOLEY_CounterHit"); 
                    SetState(PlayerStates.Hit);
                    spinHitParticle.Play();
                    rigidbody.AddForce(-relativePosition * mediumAttack, ForceMode.Impulse); // Applies force from the player punching
                    pc.Vibrate(tempObject.GetComponent<PlayerController>().playerNumber, 1.0f, 1.0f, .1f); // Makes the controller vibrate
                    Vibrate(playerNumber, 1.0f, 1.0f, .1f);
                    hitCounter++;
                    pc.contact = true; // Stops punch from happening more than once

                    tempObject.GetComponent<PlayerController>().counters++; // Increases counters

                    return;
                }                      
                
                //Plays SFX Punch
                punchSFX(punchType); 
               

                Vector3 rotationDirection = transform.position - tempObject.transform.position;
                Vector3 rotation = new Vector3(rotationDirection.x, 0.0f, rotationDirection.z);

                pc.targetState = PlayerStates.Hit;
                tempObject.transform.rotation = Quaternion.LookRotation(rotation);
                pc.hitCounter++;
                CheckForParticle().Play(true);
                tempObject.rigidbody.AddForce(relativePosition * force, ForceMode.Impulse); // Applies force
                pc.SetState(PlayerStates.Hit);                
                pc.Vibrate(tempObject.GetComponent<PlayerController>().playerNumber, 1.0f, 1.0f, .1f); // Makes the controller vibrate
                TrackPlayerHits(tempObject.name); // Increases most hit

                hits++; // Increases the number of hits the player has landed
                
                Vibrate(playerNumber, 1.0f, 1.0f, .1f);
                
                if(pc.hasObject)
                    pc.DropObject();
                
                punchTime += increasedPunchTime;
                contact = true;
            }

            else if (tempObject.CompareTag("Tray"))
            {
                rigidbody.velocity = Vector3.zero;
                contact = true;

                if(!PlayersTray())
                {
                    Tray tTray = tempObject.GetComponent<Tray>();
                    tTray.tray.SetBool("hit", true);
                    tTray.lid.SetBool("hit", true);
                    tTray.hits++;
                    tTray.hitReference = transform.name;

                    //Plays SFX, depending on if the tray has an object, it will play different sounds
                    AudioController.Play (tempObject.GetComponent<Tray>().hasObject ? "GLASS_Crack" :"SFX_trayPlaced"); 

                    tempObject = null;
                    punchTime += increasedPunchTime;
                    return;
                }
            }                            
        }
    }

    public bool PlayersTray()
    {
        return tray.trayColor == tempObject.GetComponent<Tray>().trayColor ? true : false;
    }
    
    float CalculateAttackPower()
    {
        ///////////////////////////////////
        /// Punch Profile
        //////////////////////////////////
        
        float d1 = 1.4f;
        float d2 = 1.75f;
        Vector3 distance = transform.position - tempObject.transform.position;
        
        float nDistance = distance.magnitude;
        
        if (nDistance <= d1)
        {
            force = weakAttack;
            punchType = weakPunch;
            return force;
        } 
        else if (nDistance >= d1 && nDistance <= d2)
        {
            force = mediumAttack;
            punchType = mediumPunch;
            return force;
        }
        else
        {
            force = weakAttack;
            punchType = weakPunch;
            return force;
        }
    }

    bool CannotHit() // If the player being hit is in the hit or critical hit state they will not be able to be hit again
    {
        return tempObject.GetComponent<PlayerController>().currentState == PlayerController.PlayerStates.Hit ||
                tempObject.GetComponent<PlayerController>().currentState == PlayerController.PlayerStates.CriticalHit
                ? true : false; 
    }

    bool HitSpin()
    {
        return tempObject.GetComponent<PlayerController>().spinning ? true : false; // Checks if the player hit someone spinning
    }

    ParticleSystem CheckForParticle()
    {
        if (tempObject.GetComponent<PlayerController>().color == color)
            return friendPunchParticle;
        if (!tempObject.GetComponent<PlayerController>().hasObject)
        {
            if(tempObject.GetComponent<PlayerController>().hitCounter <= 2)
                return punchParticle;
            else
            {
                dazes++;
                return dazePunchParticle;
            }
        }
        else
            return ingredientPunchParticle;
    }

    public void TrackPlayerHits(string name)
    {
        switch (name)
        {
            case "Player 1":
            {
                johnnyHit++;
                return;
            }
            case "Player 2":
            {
                sophiaHit++;
                return;
            }
            case "Player 3":
            {
                gingerHit++;
                return;
            }
            case "Player 4":
            {
                louisHit++;
                return;
            }
        }
    }
    
    #endregion
    
    #region PickUP
    
    // When the player presses the button the game will search for an object to pick up
    // The game will only let the player pick up one object
    public void PickUpObject()
    {
        Picking pick = rangeTrigger.GetComponent<Picking>();
        if(pick.objs.Count > 0)
        {
            float objectDistance = 10f;

            foreach (var p in pick.objs)
            {
                float dis = Vector3.Distance(transform.position, p.transform.position);
                if (dis < objectDistance)
                {
                    objectDistance = dis;
                    pickObject = p;
                }
            }

            if(pickObject != null)
            {
                if(pickObject.GetComponent<Objects>() != null)
                {
                    Objects pickedUpObject = pickObject.GetComponent<Objects>();
                    if (pickedUpObject.obtainable)
                    {
                        animator.SetBool("ingredient", true); // Makes animation tree use the ingredient animations
                        animator.SetBool("pickUp", false);       

                        Vector3 objectPosition = transform.position + (Vector3.up * 3);                

                        Quaternion objectRotation = transform.rotation;
                        
                        // Sets the parent reference for th goals class
                        pickObject.GetComponent<IngredientGeneric>().parentReference = transform.name;
                        
                        
                        // Makes the object a child and sets it to not move or rotate
                        pickObject.GetComponent<Objects>().shouldRotate = false;
                        pickObject.GetComponent<Objects>().obtainable = false;
                        pickObject.rigidbody.useGravity = false;
                        pickObject.rigidbody.isKinematic = true;
                        pickObject.collider.isTrigger = true;
                        pickObject.transform.parent = transform;
                        pickObject.transform.position = objectPosition;
                        pickObject.transform.rotation = objectRotation;
                        // pickObject.transform.localScale = Vector3.one;
                        canPick = false;
                        hasObject = true;

                        ChangeTarget(true); // Turns the particle target on

                        if(tray.ingredientCount == 0)
                        {
                            for(int i = 0; i < tray.ingredients.Count; i++)
                            {if(tray.ingredients[i].Contains(pickObject.name.Substring(0, 5)))ShowArrow();}
                        }

                    }
                }
            }
        }
    }
    
    // This is called when the player wants to place an object onto the tray
    public void PlaceObject()
    {
        if(pickObject != pickRef)
        {
            // Restores the object to using gravity and collidable
            pickObject.rigidbody.useGravity = true;
            pickObject.rigidbody.isKinematic = false;
            pickObject.transform.parent = GameObject.Find("Ingredients").transform;
            pickObject.collider.isTrigger = false;
            
            // Places the object in front of the player
            pickObject.transform.position += transform.forward * 2;
            ChangeTarget(false); // Turns off target on tray
            TurnArrowOff();
            // Returns the object reference to an empty game object
            pickObject = pickRef;
            
            throwObjectVO();         

            canPick = true;
            hasObject = false;
            animator.SetBool("ingredient", false);
            animator.SetBool("throw", false);
            SetState(PlayerStates.Idle);
       }
    }
    
    // When the player dies or gets hit too many times 
    // they will drop the item they are holding
    void DropObject()
    {
        //Checks to see if the player has an object
        if (pickObject != pickRef)
        {
            Vector3 newPos = (Vector3.up * 3);
            float dropTime = .5f;

            TurnArrowOff();
            ChangeTarget(false); // Turns off target on trays 

            // Throws the object into the air and sets it to its original state
            pickObject.rigidbody.useGravity = true;
            pickObject.rigidbody.isKinematic = false;
            pickObject.rigidbody.AddForce(dropDirection * 5, ForceMode.Impulse);
            pickObject.transform.position += Vector3.Lerp(transform.position, newPos, dropTime);
            pickObject.transform.parent = GameObject.Find("Ingredients").transform;
            pickObject.collider.isTrigger = false;
//            pickObject.transform.localScale = Vector3.one;
            pickObject = pickRef;
            
            dropObjectVO();
            
            hasObject = false;
            canPick = true;
            animator.SetBool("ingredient", false);
        }
    }

    void ShowArrow()
    {
        string blue = "blue";
        if (color.Contains(blue) && !tray.hasObject)
            GameModes.GetInstance.leftArrow.gameObject.SetActive(true);
        else if(!tray.hasObject)
            GameModes.GetInstance.rightArrow.gameObject.SetActive(true);
    }

    void TurnArrowOff()
    {
        string blue = "blue";
        if (color.Contains(blue))
            GameModes.GetInstance.leftArrow.gameObject.SetActive(false);
        else
            GameModes.GetInstance.rightArrow.gameObject.SetActive(false);
    }

    public void ClearPicks()
    {
        rangeTrigger.GetComponent<Picking>().objs.Clear();
    }

    void ChangeTarget(bool turnOn)
    {
        if (turnOn)
        {
            if (tray.CheckIngredient(pickObject.name.Substring(0, 5)))
                tray.target.gameObject.SetActive(true);
        }
        else
        {
            if (tray.CheckIngredient(pickObject.name.Substring(0, 5)))
                tray.target.gameObject.SetActive(false);
        }
    }

    public void GetTray(string color)
    {
        string pink = "pink";
        tray = color.Contains(pink) ? GameObject.Find("Tray_Right").GetComponent<Tray>() : GameObject.Find("Tray_Left").GetComponent<Tray>();
    }
    
    #endregion

    #region Sounds
    
    protected virtual void punchingVO(){}

    protected virtual void punchedVO(){}

    protected virtual void stageOffVO(){}

    protected virtual void dropObjectVO(){}

    protected virtual void respawnVO(){}

    protected virtual void throwObjectVO(){}

    protected virtual void cheerVO(){}
    protected virtual void guitarUseVO(){}
    protected virtual void dazedVO(){}
    //Foley Sound Effects
    //public  virtual void slideSFX(){AudioController.Play ("SFX_skid");}
    void punchSFX(string SFXName)
    {
        AudioController.Play(SFXName);
        return;
    }   
    
    #endregion 
}
