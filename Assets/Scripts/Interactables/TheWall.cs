using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;
[ExecuteAlways]
//yes
public class TheWall : MonoBehaviour
{
    public UnityEvent OnDestroy;
    [SerializeField] int columns;
    [SerializeField] int rows;
    [SerializeField] GameObject wallCubePrefab;
    [SerializeField] GameObject socketWallPrefab;

    [SerializeField] int socketPosition = 1;
    [SerializeField] XRSocketInteractor wallSocket;
    public XRSocketInteractor GetWallSocket => wallSocket;
    [SerializeField] ExplosiveDevice explosiveDevice;
    [SerializeField] List<GenerateColumn> generatedColumn;
    GameObject[] wallCubes;

    [SerializeField] float cubeSpacing = 0.005f;

    private Vector3 cubeSize;

    private Vector3 spawnPosition;

    [SerializeField] bool buildWall;
    [SerializeField] bool deleteWall;
    [SerializeField] bool destroyWall;
    [SerializeField] int maxPower;
    [SerializeField] AudioClip destroyWallClip;

    public AudioClip GetDestroyClip => destroyWallClip;

    [SerializeField] AudioClip socketClip;

    public AudioClip GetSocketClip => socketClip;
    // Start is called before the first frame update
    void Start()
    {
        if (wallSocket != null)
        {
            wallSocket.onSelectEntered.AddListener(OnSocketEnter);
            wallSocket.onSelectExited.AddListener(OnSocketExited);
        }
        if (explosiveDevice != null)
        {
            explosiveDevice.OnDetonated.AddListener(OnDestroyWall);
        }

    }

    private void BuildWall()
    {
        if (wallCubePrefab != null)
        {
            cubeSize = wallCubePrefab.GetComponent<MeshRenderer>().bounds.size;
        }
        spawnPosition = transform.position;
        int socketedColumn = Random.Range(0, columns);

        for (int i = 0; i < columns; i++)
        {
            if (i == socketedColumn)
            {
                GenerateColumn(i, rows, true);
            }
            else
            {
                GenerateColumn(i, rows, false);
            }
            spawnPosition.x += cubeSize.x + cubeSpacing;

        }
    }

    private void GenerateColumn(int index, int height, bool socketed)
    {
        GenerateColumn tempColumn = new GenerateColumn();

        tempColumn.InitializeColumn(transform, index, height, socketed);
        spawnPosition.y = transform.position.y;

        wallCubes = new GameObject[height];

        for (int i = 0; i < wallCubes.Length; i++)
        {
            if (wallCubePrefab != null)
            {
                wallCubes[i] = Instantiate(wallCubePrefab, spawnPosition, transform.rotation);
                tempColumn.SetCube(wallCubes[i]);
            }
            spawnPosition.y += cubeSize.y + cubeSpacing;
        }

        if (socketed && socketWallPrefab != null)
        {
            if (socketPosition < 0 || socketPosition >= height)
            {
                socketPosition = 0;
            }
            AddSocketWall(tempColumn);


        }
        generatedColumn.Add(tempColumn);
    }

    private void AddSocketWall(GenerateColumn socketColumn)
    {
        if (wallCubes[socketPosition] != null)
        {
            Vector3 saveTransform = wallCubes[socketPosition].transform.position;
            DestroyImmediate(wallCubes[socketPosition]);
            wallCubes[socketPosition] = Instantiate(socketWallPrefab, saveTransform, transform.rotation);
            socketColumn.SetCube(wallCubes[socketPosition]);

            if (socketPosition == 0)
            {
                wallCubes[socketPosition].transform.SetParent(transform);
            }
            else
            {
                wallCubes[socketPosition].transform.SetParent(wallCubes[0].transform);
            }
            wallSocket = wallCubes[socketPosition].GetComponentInChildren<XRSocketInteractor>();
            if (wallSocket != null)
            {
                wallSocket.onSelectEntered.AddListener(OnSocketEnter);
                wallSocket.onSelectExited.AddListener(OnSocketExited);
            }
        }
    }
    private void OnSocketExited(XRBaseInteractable arg0)
    {
        if (generatedColumn.Count >= 1)
        {
            for (int i = 0; i < generatedColumn.Count; i++)
            {
                generatedColumn[i].ResetColumn();
            }
        }
    }
    private void OnSocketEnter(XRBaseInteractable arg0)
    {
        if (generatedColumn.Count >= 1)
        {
            for (int i = 0; i < generatedColumn.Count; i++)
            {
                generatedColumn[i].ActivateColumn();
            }
        }
    }
    private void OnDestroyWall()
    {
        if (generatedColumn.Count >= 1)
        {
            for (int i = 0; i < generatedColumn.Count; i++)
            {
                int power = Random.Range(maxPower / 2, maxPower);
                generatedColumn[i].DestroyColumn(power);
            }
        }

        OnDestroy?.Invoke();

    }


    // Update is called once per frame
    void Update()
    {
        if (buildWall)
        {
            buildWall = false;
            //testing
            BuildWall();
        }

        if (deleteWall)
        {
            deleteWall = false;
            for (int i = 0; i < generatedColumn.Count; i++)
            {
                generatedColumn[i].DeleteColumn();
            }
            if (generatedColumn.Count > 0)
            {
                generatedColumn.Clear();
            }
        }

    }
}

[System.Serializable]
public class GenerateColumn
{
    [SerializeField] GameObject[] wallCubes;
    [SerializeField] bool isSocketed;
    [SerializeField] int columnIndex;
    private bool isParented;
    private Transform parentObject;
    private Transform columnObject;
    private const string Column_name = "Column";
    private const string Socketed_Column_name = "SocketedColumn";

    public void InitializeColumn(Transform parent, int index, int rows, bool socketed)
    {
        columnIndex = index;
        parentObject = parent;
        wallCubes = new GameObject[rows];
        isSocketed = socketed;
    }
    public void SetCube(GameObject cube)

    {
        for (int i = 0; i < wallCubes.Length; i++)
        {
            if (!isParented)
            {
                isParented = true;
                SetColumnName(cube, columnIndex);
                cube.transform.SetParent(parentObject);
                columnObject = cube.transform;

            }
            else
            {
                cube.transform.SetParent(columnObject);
            }
            if (wallCubes[i] == null)
            {
                wallCubes[i] = cube;
                break;

            }
        }
    }

    private void SetColumnName(GameObject column, int index)
    {
        if (isSocketed)
        {
            column.name = Socketed_Column_name;
        }
        else
        {
            column.name = Column_name;
        }
        column.name += index.ToString();
    }
    public void DeleteColumn()
    {
        for (int i = 0; i < wallCubes.Length; i++)
        {
            if (wallCubes[i] != null)
            {
                Object.DestroyImmediate(wallCubes[i]);
            }
        }
        wallCubes = new GameObject[0];
    }
    public void DestroyColumn(int power)
    {
        foreach (GameObject cube in wallCubes)
        {
            Rigidbody rb = cube.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.constraints = RigidbodyConstraints.None;
            cube.transform.SetParent(parentObject);
            rb.AddRelativeForce(Random.onUnitSphere * power);
        }
        // OnSocketEnterEvent?.Invoke();
    }

    public void ActivateColumn()
    {
        foreach (GameObject cube in wallCubes)
        {
            Rigidbody rb = cube.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            // rb.GetComponent<Material>().color = Color.red;
        }
    }

    public void ResetColumn()
    {
        foreach (GameObject cube in wallCubes)
        {
            Rigidbody rb = cube.GetComponent<Rigidbody>();
            rb.isKinematic = true;
        }
    }

}