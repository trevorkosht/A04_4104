using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("Tutorial Section")]
    public GameObject tutorialRoomPrefab;
    public GameObject tutorialTunnelPrefab;

    [Header("Random Room/Tunnel Pools")]
    public GameObject[] roomPrefabs;
    public GameObject[] tunnelPrefabs;

    [Header("Generation Settings")]
    public int roomsToGenerate = 3;
    public int maxBranchesPerRoom = 2; 
    public float roomSpacing = 30f;

    private List<Vector3> occupiedPositions = new List<Vector3>();
    private int roomsRemaining;

    void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        roomsRemaining = roomsToGenerate;

        Vector3 pos = Vector3.zero;
        occupiedPositions.Add(pos);

        // Tutorial Room
        Instantiate(tutorialRoomPrefab, pos, Quaternion.identity);

        // Tutorial Tunnel (forward only)
        pos += Vector3.forward * roomSpacing;
        Instantiate(tutorialTunnelPrefab, pos, Quaternion.identity);
        occupiedPositions.Add(pos);

        // First main room (forward only)
        pos += Vector3.forward * roomSpacing;
        GameObject room1 = InstantiateRandomRoom(pos);
        occupiedPositions.Add(pos);
        roomsRemaining--;

        // Now recursively branch from room1
        GenerateBranches(pos);
    }

    void GenerateBranches(Vector3 parentPos)
    {
        if (roomsRemaining <= 0)
            return;

        int branches = Random.Range(1, maxBranchesPerRoom + 1);
        branches = Mathf.Min(branches, roomsRemaining);

        for (int i = 0; i < branches; i++)
        {
            Vector3 dir = GetRandomValidDirection(parentPos);
            Vector3 nextPos = parentPos + dir * roomSpacing;

            // Tunnel between parent and child
            Vector3 tunnelPos = parentPos + dir * (roomSpacing * 0.5f);
            InstantiateRandomTunnel(tunnelPos);
            occupiedPositions.Add(tunnelPos);

            // Create room
            GameObject newRoom = InstantiateRandomRoom(nextPos);
            occupiedPositions.Add(nextPos);
            roomsRemaining--;

            // Recursively allow this room to branch too
            GenerateBranches(nextPos);

            if (roomsRemaining <= 0)
                break;
        }
    }

    // -------------------------------------------------------
    // Helpers
    // -------------------------------------------------------

    Vector3 GetRandomValidDirection(Vector3 parentPos)
    {
        Vector3[] dirs = new Vector3[]
        {
            Vector3.forward,
            Vector3.back,
            Vector3.right,
            Vector3.left
        };

        Shuffle(dirs);

        foreach (var d in dirs)
        {
            Vector3 p = parentPos + d * roomSpacing;
            if (!occupiedPositions.Contains(p))
                return d;
        }

        // very rare fallback if no direction is free
        return Vector3.forward;
    }

    GameObject InstantiateRandomRoom(Vector3 pos)
    {
        int i = Random.Range(0, roomPrefabs.Length);
        return Instantiate(roomPrefabs[i], pos, Quaternion.identity);
    }

    GameObject InstantiateRandomTunnel(Vector3 pos)
    {
        int i = Random.Range(0, tunnelPrefabs.Length);
        return Instantiate(tunnelPrefabs[i], pos, Quaternion.identity);
    }

    void Shuffle(Vector3[] arr)
    {
        for (int i = arr.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (arr[i], arr[j]) = (arr[j], arr[i]);
        }
    }
}
