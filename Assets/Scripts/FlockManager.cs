using UnityEngine;
using System.Collections.Generic;
using UnityEditor;


public class FlockManager : MonoBehaviour
{
    public Boid boidPrefab;
    public int boidCount = 100;
    public Vector2 spawnBounds = new Vector2(10, 10);

    private List<Boid> allBoids = new List<Boid>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for(int i = 0; i < boidCount; i++)
        {
            Vector2 pos = new Vector2(Random.Range(-spawnBounds.x, spawnBounds.x), Random.Range(-spawnBounds.y, spawnBounds.y));

            Boid boid = Instantiate(boidPrefab, pos, Quaternion.identity);
            boid.manager = this;
            allBoids.Add(boid);
        }
    }

    public List<Boid> GetNeighbors(Boid boid, float radius)
    {
        List<Boid> neighbors = new List<Boid>();
        foreach(Boid other in allBoids)
        {
            if (other == boid) continue;
            if (Vector2.Distance(boid.transform.position, other.transform.position) <= radius)
            {
                neighbors.Add(other);
            } 
        }
        return neighbors;
    }
}
