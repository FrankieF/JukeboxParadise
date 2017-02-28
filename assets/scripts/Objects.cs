using UnityEngine;
using System.Collections;

public class Objects : MonoBehaviour
{	
	public bool shouldRotate; // Tells objects to rotate
    public bool obtainable; // Allows objects to be picked up
    public float rotateSpeed = .35f; // How fast they should rotate
    public float timeRemaining = 10f, resetTime = 10f; // The amount of time the ingredient stays on the record
    public int number = 0;
    private static float gravity = -0.98f; 
    public string ingredient = "Ingredient";
    public ParticleSystem spawnParticle = new ParticleSystem();
	
    void OnEnable(){timeRemaining = resetTime;} // Resets the time 

	// Update is called once per frame
	void Update ()
	{
        if(Time.timeScale != 0) // If the game becomes paused the objects will stop rotating
        {
    		if(shouldRotate)	
    		{
    			transform.RotateAround(Vector3.zero,Vector3.up, rotateSpeed); // Rotates around center of world
                timeRemaining -= Time.deltaTime;
    		}
    		else if(!transform.rigidbody.isKinematic && !shouldRotate)
    		{
    			transform.rigidbody.velocity += new Vector3(0f,gravity); // Applies gravity
            }
            if(timeRemaining <= 0)
                StartCoroutine(Return());
        }		
	}

    IEnumerator Return()
    {
        float t = 0f, time = 5f;

        while(true)
        {
            if(t <= time)
            {
                if(transform.parent.name != (ingredient + "s"))
                {
                    timeRemaining = 2f;
                    yield break;
                }

                t += Time.deltaTime;
                transform.renderer.materials[number].SetFloat("_Shine", (t * 7));
            }
            else
            {
                GetComponent<IngredientGeneric>().ReturnToLocation();
                transform.renderer.materials[number].SetFloat("_Shine", 0);
            }
            yield return null;
        }
    }

	void OnCollisionEnter(Collision col)
    {       
        if (col.gameObject.name == "Record" && transform.parent.name=="Ingredients")  // Plays sound when object hits record
            AudioController.Play ("fruitsHittingRecordSFX"); 
    }

	void OnCollisionStay(Collision col)
	{
		if(col.gameObject.name == "Record" && transform.parent.name == "Ingredients")
		{
            GetComponent<IngredientGeneric>().parentReference = ""; // Resets parent reference
			shouldRotate = true; // Tells the object to rotate
			transform.rigidbody.velocity = new Vector3(0f,0f,0f); // Stops velocity
            obtainable = true; // Allows objects to be picked up

            if(timeRemaining <= 0)
                timeRemaining = 1f;
		}
	}
}
