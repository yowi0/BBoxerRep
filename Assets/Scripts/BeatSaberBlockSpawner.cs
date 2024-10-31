using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class BeatSaberBlockSpawner : MonoBehaviour
{
    public GameObject blueBlockPrefab;
    public Transform spawnPoint;

    public float horizontalSpacing = 2.0f;
    public float verticalSpacing = 2.0f;

    public string beatMapFilePath;

    public float bpm = 105;

    public float noteJumpMovementSpeed = 10f;
    public float noteJumpStartBeatOffset = 1f;

    private void Start()
    {
        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        string path = Path.Combine(Application.streamingAssetsPath, beatMapFilePath);
        yield return StartCoroutine(ReadFileFromStreamingAssets(path, (jsonString) =>
        {
            if (!string.IsNullOrEmpty(jsonString))
            {
                BeatSaberMapData mapData = JsonUtility.FromJson<BeatSaberMapData>(jsonString);
                StartCoroutine(SpawnBlocks(mapData));
            }
        }));
    }

    private IEnumerator ReadFileFromStreamingAssets(string filePath, System.Action<string> onComplete)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            Debug.LogError("File path is empty or null.");
            onComplete?.Invoke(null);
            yield break;
        }

        using (UnityWebRequest www = UnityWebRequest.Get(filePath))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error reading file: " + www.error);
                onComplete?.Invoke(null);
            }
            else
            {
                onComplete?.Invoke(www.downloadHandler.text);
            }
        }
    }

    private IEnumerator SpawnBlocks(BeatSaberMapData mapData)
    {
        float startTime = Time.time;

        foreach (BeatSaberBlockData blockData in mapData._notes)
        {
            float targetTime = (blockData._time / bpm) * 60;
            float timeToWait = targetTime - (Time.time - startTime) + noteJumpStartBeatOffset;

            if (timeToWait > 0)
            {
                yield return new WaitForSeconds(timeToWait);
            }

            // Solo spawneamos bloques azules
            if (blockData._type == (int)BeatSaberBlockType.Blue)
            {
                SpawnBlock(blockData);
            }
        }
    }

    private void SpawnBlock(BeatSaberBlockData blockData)
    {
        GameObject blockPrefab = blueBlockPrefab;

        // Definir una altura fixa més baixa per a la generació
        float fixedVerticalPosition = 0.5f; // Ajusta aquest valor si cal

        // Generar una posició aleatòria dins d'un rang especificat
        Vector3 randomPosition = spawnPoint.position + new Vector3(
            Random.Range(-horizontalSpacing, horizontalSpacing),
            fixedVerticalPosition, // Utilitza una posició vertical fixa
            0); // Manté la profunditat constant

        GameObject blockInstance = Instantiate(blockPrefab, randomPosition, Quaternion.identity);
        
        // Opcional: Ajusta la velocitat de moviment del bloc si es necessita
        BlockController blockController = blockInstance.GetComponent<BlockController>();
        blockController.moveSpeed = noteJumpMovementSpeed; // Utilitza el mateix valor de velocitat de moviment
    }

    [System.Serializable]
    public class BeatSaberMapData
    {
        public BeatSaberBlockData[] _notes;
    }

    [System.Serializable]
    public class BeatSaberBlockData
    {
        public float _time;
        public int _lineIndex;
        public int _lineLayer;
        public int _type;
    }

    public enum BeatSaberBlockType
    {
        Red, Blue, Bomb
    };
}
