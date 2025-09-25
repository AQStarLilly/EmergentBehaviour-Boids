using UnityEngine;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;

public class Boid : MonoBehaviour
{
    public float speed = 2f;
    public float neighborRadius = 3f;
    public float seperationRadius = 1f;

    [HideInInspector] public FlockManager manager;
    private Vector2 velocity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        velocity = Random.insideUnitCircle.normalized * speed;
    }

    // Update is called once per frame
    void Update()
    {
        List<Boid> neighbors = manager.GetNeighbors(this, neighborRadius);

        //Calculate rule forces
        Vector2 seperation = Vector2.zero;
        Vector2 alignment = Vector2.zero;
        Vector2 cohesion = Vector2.zero;

        if(neighbors.Count > 0)
        {
            Vector2 avgPosition = Vector2.zero;
            Vector2 avgVelocity = Vector2.zero;
            
            foreach (Boid neighbor in neighbors)
            {
                float distance = Vector2.Distance(transform.position, neighbor.transform.position);

                //Seperation
                if(distance < seperationRadius)
                {
                    seperation += (Vector2)(transform.position - neighbor.transform.position);
                }

                avgPosition += (Vector2)neighbor.transform.position;
                avgVelocity += neighbor.velocity;
            }

            avgPosition /= neighbors.Count;
            avgVelocity /= neighbors.Count;

            //Cohesion
            cohesion = (avgPosition - (Vector2)transform.position);

            //Alignment
            alignment = avgVelocity;
        }

        //Blend Rules
        Vector2 acceleration = seperation * 1.15f + alignment * 1f + cohesion * 1f;
        velocity += acceleration * Time.deltaTime;
        velocity = velocity.normalized * speed;

        //Move
        transform.position += (Vector3)(velocity * Time.deltaTime);

        //Rptate boid in direction of travel
        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }

    public Vector2 GetVelocity() => velocity;
}
