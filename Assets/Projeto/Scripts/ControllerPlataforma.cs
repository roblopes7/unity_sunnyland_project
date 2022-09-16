using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerPlataforma : MonoBehaviour
{
    [SerializeField]
    public Transform plataforma, pontoA, pontoB;
    [SerializeField]
    public float velocidadePlataforma;
    [SerializeField]
    public Vector3 pontoDestino;
    [SerializeField]
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        plataforma.position = pontoA.position;
        pontoDestino = pontoB.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(plataforma.position == pontoA.position)
        {
            pontoDestino = pontoB.position;
        }
        if(plataforma.position == pontoB.position)
        {
            pontoDestino = pontoA.position;
        }

        plataforma.position = Vector3.MoveTowards(plataforma.position, pontoDestino, velocidadePlataforma);
    }
}
