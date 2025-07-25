using UnityEngine;

public class DebugController : MonoBehaviour
{
    public static DebugController Instance { get; private set; }

    public float smoothMoveTime = 20f;

    private Vector3Int currentChunkPosition = Vector3Int.zero;
    private Vector3Int lastChunkPosition = Vector3Int.zero;
    private Vector3Int input;

    private float moveCooldownTimer = 0f;
    public float moveCooldown = 0.2f;

    private Vector3 targetPosition;

    private WorldSettings ws => WorldGenerator.Settings;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Update()
    {
        moveCooldownTimer += Time.deltaTime;

        HandleInput();

        MoveToTargetPosition();
    }

    private void MoveToTargetPosition()
    {
        if (currentChunkPosition != lastChunkPosition)
        {
            if (ChunkGenerator.Chunks.TryGetValue(currentChunkPosition, out Chunk chunk))
            {
                if (input != Vector3Int.zero)
                {
                    targetPosition = chunk.transform.position;
                }
            }
        }

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothMoveTime);
        lastChunkPosition = currentChunkPosition;
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            WorldGenerator.Instance.GenerateWorld();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (ChunkGenerator.Chunks.TryGetValue(currentChunkPosition, out Chunk chunk))
            {
                Debug.Log($"Current chunk: {chunk.GetType().Name} at {currentChunkPosition}");

                DensityDisplayer.Instance.ToggleDensities(chunk);
            }
        }

        input = Vector3Int.zero;

        if (Input.GetKey(KeyCode.W)) input.z += 1;
        if (Input.GetKey(KeyCode.S)) input.z -= 1;

        if (Input.GetKey(KeyCode.D)) input.x += 1;
        if (Input.GetKey(KeyCode.A)) input.x -= 1;

        if (Input.GetKey(KeyCode.E)) input.y += 1;
        if (Input.GetKey(KeyCode.Q)) input.y -= 1;

        if (moveCooldownTimer < moveCooldown)
        {
            return; // Prevent movement if cooldown is active
        }

        if (input != Vector3Int.zero)
        {
            if (!ChunkGenerator.Chunks.ContainsKey(currentChunkPosition + input))
            {
                return;
            }

            currentChunkPosition += input;

            moveCooldownTimer = 0f; // Reset cooldown timer
        }
    }

    void OnDrawGizmos()
    {
        if (!WorldGenerator.Instance) { return; }
        if (!WorldGenerator.IsReady) { return; }

        Gizmos.color = new Color(1f, 1f, 1f, 0.4f);

        Gizmos.DrawWireCube(transform.position, Vector3.one * ws.chunkSize / 4);
    }
}