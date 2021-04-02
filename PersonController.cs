using UnityEngine;
using System.Timers;
using System;

//Adding this allows us to access members of the UI namespace including Text.
using UnityEngine.UI;
using System.Threading;

public class PersonController : MonoBehaviour
{

	public float speed;             //Floating point variable to store the player's movement speed.
	public Text countText;          //Store a reference to the UI Text component which will display the number of pickups collected.
	public Text winText;            //Store a reference to the UI Text component which will display the 'You win' message.
	public Text lifeText;
	public Text collisionText;
	public UnityEngine.UI.Image img;

	private DateTime virusCollision;
	private DateTime previousVirusCollision;

	private Rigidbody2D rb2d;       //Store a reference to the Rigidbody2D component required to use 2D Physics.
	private int count;              //Integer to store the number of pickups collected so far.
	private int life;

	private int totalCount = 9;

	// Use this for initialization
	void Start()
	{
		img.gameObject.SetActive(false);
		//Get and store a reference to the Rigidbody2D component so that we can access it.
		rb2d = GetComponent<Rigidbody2D>();

		//Initialize count to zero.
		count = 0;

		var render = new SpriteRenderer();
		//
		virusCollision = DateTime.UtcNow;
		previousVirusCollision = DateTime.UtcNow;

		//Initialze winText to a blank string since we haven't won yet at beginning.
		winText.text = "";

		//Call our SetCountText function which will update the text with the current value for count.
		countText.text = "Count: " + count.ToString() + " / " + totalCount + "\t" + "Collect all vaccines in order to win the stage"; ;

		// Initialize life to 2, which means the player starts with a mask to defend agains the first virus
		life = 2;
		lifeText.text = "";
		SetLifeText();

		speed = 12;
	}

	//FixedUpdate is called at a fixed interval and is independent of frame rate. Put physics code here.
	void FixedUpdate()
	{
		if (DateTime.UtcNow > previousVirusCollision)
		{
			if ((DateTime.UtcNow - previousVirusCollision).Seconds > 2)
			{
				//collisionText.text = "";
				img.gameObject.SetActive(false);
				previousVirusCollision = DateTime.UtcNow;
			}
		}
		else collisionText.text = "";
		//Store the current horizontal input in the float moveHorizontal.
		float moveHorizontal = Input.GetAxis("Horizontal");

		//Store the current vertical input in the float moveVertical.
		float moveVertical = Input.GetAxis("Vertical");

		//Use the two store floats to create a new Vector2 variable movement.
		Vector2 movement = new Vector2(moveHorizontal, moveVertical);

		//Call the AddForce function of our Rigidbody2D rb2d supplying movement multiplied by speed to move our player.
		rb2d.AddForce(movement * speed);
		rb2d.velocity = new Vector2(moveHorizontal * speed, moveVertical * speed);
        rb2d.rotation = 0;
	}

	//OnTriggerEnter2D is called whenever this object overlaps with a trigger collider.
	void OnTriggerEnter2D(Collider2D other)
	{
		

		bool mask = other.gameObject.CompareTag("Mask");
		bool virus = other.gameObject.CompareTag("Virus");
		bool vaccine = other.gameObject.CompareTag("Vaccine");

		//Check the provided Collider2D parameter other to see if it is tagged "PickUp", if it is...
		if (vaccine || virus || mask)
		{
			//... then set the other object we just collided with to inactive.
			other.gameObject.SetActive(false);

            if (vaccine)
            {
				//Add one to the current value of our count variable.
				count = count + 1;

				//Update the currently displayed count by calling the SetCountText function.
				SetCountText();
			}
            else if (virus)
            {
				life = life - 1;
				SetLifeText();
				//collisionText.text = "Aye Corona (;ﾟ︵ﾟ;)";
				//Thread.Sleep(20);
				//virusCollision = DateTime.UtcNow;
				img.gameObject.SetActive(true);
			}
			else if(mask)

			{
				life = life + 1;
				SetLifeText();
			}

		}
	}
	void OnCollisionStay(Collision collisionInfo)
	{

		rb2d.rotation = 0;
	}
	void OnCollisionEnter()
    {
		rb2d.rotation = 0;

	}
    
	//This function updates the text displaying the number of objects we've collected and displays our victory message if we've collected all of them.
	void SetCountText()
	{
		//Set the text property of our our countText object to "Count: " followed by the number stored in our count variable.
		countText.text = "Count: " + count.ToString() + " / " + totalCount;

		//Check if we've collected all 9 pickups. If we have...
		if (count >= totalCount)
			//... then set the text property of our winText object to "You win!"
			winText.text = "You win!";
	}

	void SetLifeText()
	{
		//Set the text property of our our countText object to "Count: " followed by the number stored in our count variable.
		lifeText.text = "Life: " + life.ToString();

		//Check if life is zero
		if (life == 0)
        {
			//... then set the text property of our winText object to "You win!"
			winText.text = "You lose!";
			collisionText.text = "";
			speed = 0;
			rb2d.velocity = new Vector2(0, 0);
		}
			
	}
}
