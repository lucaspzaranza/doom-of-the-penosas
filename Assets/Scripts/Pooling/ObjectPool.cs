using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;

    public Dictionary<string, Queue<GameObject>> objectPool = new Dictionary<string, Queue<GameObject>>();

    [SerializeField] private List<GameObject> prefabsList;
    [SerializeField] private int poolInitialSize = 10;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        
        foreach (var queue in objectPool)
        {
            print(queue);
        }
    }

    public GameObject GetObject(GameObject gameObj)
    {
        if (objectPool.TryGetValue(gameObj.name, out Queue<GameObject> objectList))
        {
            if (objectList.Count == 0)
                return CreateNewObject(gameObj);
            else
            {
                GameObject objectToReturn = objectList.Dequeue();
                objectToReturn.SetActive(true);
                return objectToReturn;
            }
        }

        return CreateNewObject(gameObj);
    }

    private GameObject CreateNewObject(GameObject gameObj)
    {
        GameObject newGameObj = Instantiate(gameObj);
        newGameObj.name = gameObj.name;
        return newGameObj;
    }


    // A variável objectPool é um dicionário em que as chaves são os nomes dos Prefabs, e os valores
    // são filas desses objetos instanciados pra serem retirados pra pooling.
    // TryGetValue é utilizado pra verificar se no dicionário existe alguma chave com o nome daquele GameObject, ou seja,
    // pra saber se existe alguma pool daquele objeto.
    public IEnumerator InitializePool(string nameOfPrefab)
    {
        GameObject selectedPrefab = prefabsList.SingleOrDefault(prefab => prefab.name == nameOfPrefab);

        if (selectedPrefab != null) 
        {
            bool hasValue = objectPool.TryGetValue(selectedPrefab.name, out Queue<GameObject> queue);
            if (!hasValue)
            {
                queue = new Queue<GameObject>();
                objectPool.Add(nameOfPrefab, queue);
            }

            GameObject poolParent = new GameObject($"{nameOfPrefab} Pool");
            poolParent.transform.SetParent(gameObject.transform);

            for (int i = 0; i < poolInitialSize; i++)
            {
                var newPoolInstance = Instantiate(selectedPrefab, poolParent.transform);
                newPoolInstance.name = newPoolInstance.name.Replace("(Clone)", string.Empty);
                queue.Enqueue(newPoolInstance);
                newPoolInstance.SetActive(false);
                yield return new WaitForEndOfFrame();
            }
        }
    }

    public void ReturnGameObject(GameObject gameObj)
    {
        if(objectPool.TryGetValue(gameObj.name, out Queue<GameObject> objectList))
            objectList.Enqueue(gameObj);
        else
        {
            Queue<GameObject> newObjectQueue = new Queue<GameObject>();
            newObjectQueue.Enqueue(gameObj);
            objectPool.Add(gameObj.name, newObjectQueue);
        }

        gameObj.SetActive(false);
    }
}