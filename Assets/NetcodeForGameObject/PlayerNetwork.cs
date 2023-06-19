using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private Transform spawnObjectPrefab;
    [SerializeField] private float moveSpeed = 6f;
    public Color cubeColor = Color.white;
   
     public Renderer cubeRenderer;

    private void Start()
    {
        
        cubeRenderer = GetComponentInChildren<Renderer>();
    }

  private void SetCubeColor(Color color)
    {
        // cubeColor = color;

        // Call a server RPC to update the color on all clients
        UpdateCubeColorServerRpc(color);
    }

 [ServerRpc]
    private void UpdateCubeColorServerRpc(Color color)
    {
        // Update the color on all clients
        UpdateCubeColorClientRpc(color);
    }

    [ClientRpc]
    private void UpdateCubeColorClientRpc(Color color)
    {
        // Update the color on all clients
        cubeRenderer.material.color = color;
    }
    public struct MyCustomData : INetworkSerializable{
        public int _int;
        public bool _bool;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _int);
            serializer.SerializeValue(ref _bool);
        }
    }

    private NetworkVariable<MyCustomData> randomNumber = new NetworkVariable<MyCustomData>(new MyCustomData{_int = 16, _bool = true,},NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn(){
        randomNumber.OnValueChanged += (MyCustomData previousValue, MyCustomData newValue) => {
            Debug.Log(OwnerClientId + ": RandomeNumber - " + newValue._int + "; " + newValue._bool);
        };
    }

   private void Update(){

    if(!IsOwner) return;


        // Change the color of the cube for the local player
        if (Input.GetKeyDown(KeyCode.R))
        {
            SetCubeColor(Color.red);
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            SetCubeColor(Color.green);
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            SetCubeColor(Color.blue);
        }

    if(Input.GetKeyDown(KeyCode.T)){
        randomNumber.Value = new MyCustomData{_int = Random.Range(0,100), _bool = false,};
       if (IsServer){
         Transform spawned = Instantiate(spawnObjectPrefab);
        spawned.GetComponent<NetworkObject>().Spawn(true);
       }

    }

    Vector3 moveDir = new Vector3(0,0,0);
    if(Input.GetKey(KeyCode.W)) moveDir.z = -1f;
    if(Input.GetKey(KeyCode.S)) moveDir.z = +1f;
    if(Input.GetKey(KeyCode.A)) moveDir.x = +1f;
    if(Input.GetKey(KeyCode.D)) moveDir.x = -1f;
    
    
    transform.position += moveDir * moveSpeed *Time.deltaTime;
   }


    [ServerRpc]
    private void TestServerRpc(){
        Debug.Log("TestServerRpc " + OwnerClientId);
    }






}
