using UnityEngine;
using System.Collections.Generic;

public class Boid : MonoBehaviour
{
    //Movement and perception settings
    public float speed = 2f;               //Boid speed
    public float neighborRadius = 3f;      //How far a boid can see other boids
    public float seperationRadius = 1f;    //How close is to close before seperation

    [HideInInspector] public FlockManager manager;   //Reference to flock manager
    private Vector2 velocity;                        //Current direction and speed of this boid

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Give each boid a random initial velocity (random direction at start)
        velocity = Random.insideUnitCircle.normalized * speed;
    }

    /// <summary>
    /// Keeps boids inside the spawnBounds area by applying a steering force
    /// if they get to close to the edges.
    /// </summary>
    void StayInBounds()
    {
        Vector2 pos = transform.position;

        //Define boundaries from flock manager
        float minX = -manager.spawnBounds.x;
        float maxX = manager.spawnBounds.x;
        float minY = -manager.spawnBounds.y;
        float maxY = manager.spawnBounds.y;

        Vector2 steer = Vector2.zero; //Force applied if boid goes out of bounds

        //If out of bounds, push the boid back inside
        if (pos.x < minX) steer.x = 1;
        else if (pos.x > maxX) steer.x = -1;

        if (pos.y < minY) steer.y = 1;
        else if (pos.y > maxY) steer.y = -1;

        //Apply boundary force if needed
        if (steer != Vector2.zero)
        {
            velocity += steer.normalized * manager.boundaryWeight * Time.deltaTime;
        }  
    }

    // Update is called once per frame
    void Update()
    {
        //get a list of nearby boids within neighborRadius
        List<Boid> neighbors = manager.GetNeighbors(this, neighborRadius);

        //Rules vectors
        Vector2 seperation = Vector2.zero;
        Vector2 alignment = Vector2.zero;
        Vector2 cohesion = Vector2.zero;

        if(neighbors.Count > 0)
        {
            Vector2 avgPosition = Vector2.zero;   //For cohesion
            Vector2 avgVelocity = Vector2.zero;   //For alignment
            
            foreach (Boid neighbor in neighbors)
            {
                float distance = Vector2.Distance(transform.position, neighbor.transform.position);

                //Seperation rule
                //Push away from boids that are to close
                if(distance < seperationRadius)
                {
                    seperation += (Vector2)(transform.position - neighbor.transform.position);
                }

                //For cohesion and alignment
                avgPosition += (Vector2)neighbor.transform.position;
                avgVelocity += neighbor.velocity;
            }

            //Average out neighbor positions and velocities
            avgPosition /= neighbors.Count;
            avgVelocity /= neighbors.Count;

            //Cohesion rule
            //Steer toward the average position (center of mass)
            cohesion = (avgPosition - (Vector2)transform.position);

            //Alignment rule
            //Steer towards the average velocity (match heading)
            alignment = avgVelocity;
        }

        //Combine the 3 rules with weights
        Vector2 acceleration = seperation * 1.15f + alignment * 1f + cohesion * 1f;
        velocity += acceleration * Time.deltaTime;

        //Keep velocity at constant speed
        velocity = velocity.normalized * speed;

        //Move boid based on velocity
        transform.position += (Vector3)(velocity * Time.deltaTime);

        //Rotate boid in direction of travel
        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);

        //Keep boid inside boundaries
        StayInBounds();
    }

    //Getter so other boids can read this boid's velocity
    public Vector2 GetVelocity() => velocity;
}
