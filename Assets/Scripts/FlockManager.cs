using UnityEngine;
using System.Collections.Generic;
using UnityEditor;


public class FlockManager : MonoBehaviour
{
    public Boid boidPrefab;                     //Prefab reference for spawning boids
    public int boidCount = 100;                 //How many boids to spawn
    public Vector2 spawnBounds = new Vector2(10, 10);   //Area within which boids spawn

    private List<Boid> allBoids = new List<Boid>();     //Track all boids within scene

    public float boundaryWeight = 5f;    //How strongly the boids are pushed back inside bounds

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Spawn boids inside the defined spawn area
        for(int i = 0; i < boidCount; i++)
        {
            Vector2 pos = new Vector2(Random.Range(-spawnBounds.x, spawnBounds.x), Random.Range(-spawnBounds.y, spawnBounds.y));

            //Create new boid
            Boid boid = Instantiate(boidPrefab, pos, Quaternion.identity);
            boid.manager = this;     //Tell the boid who it's manager is
            allBoids.Add(boid);      //Store reference for later
        }
    }

    /// <summary>
    /// Returns a list of boids within a certain radius of the given boid. 
    /// This is used for flocking rules (seperation, alignment, cohesion)
    /// </summary>
    public List<Boid> GetNeighbors(Boid boid, float radius)
    {
        List<Boid> neighbors = new List<Boid>();
        foreach(Boid other in allBoids)
        {
            if (other == boid) continue;  //skip self
            if (Vector2.Distance(boid.transform.position, other.transform.position) <= radius)
            {
                neighbors.Add(other);
            } 
        }
        return neighbors;
    }
}
