using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public Dictionary<string, Queue<GameObject>> objectPool = new Dictionary<string, Queue<GameObject>>();

    [SerializeField] private List<GameObject> prefabsList;
    public List<GameObject> PrefabsList => prefabsList;

    [SerializeField] private int poolInitialSize = 10;

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
        Transform parent = transform.Find($"{gameObj.name} Pool");

        GameObject newGameObj = Instantiate(gameObj, parent);
        newGameObj.name = gameObj.name;
        return newGameObj;
    }


    // A vari�vel objectPool � um dicion�rio em que as chaves s�o os nomes dos Prefabs, e os valores
    // s�o filas desses objetos instanciados pra serem retirados pra pooling.
    // TryGetValue � utilizado pra verificar se no dicion�rio existe alguma chave com o nome daquele GameObject, ou seja,
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

        float xScale = gameObj.transform.localScale.x;
        float yScale = gameObj.transform.localScale.y;

        gameObj.transform.localScale = new Vector2(Mathf.Abs(xScale), Mathf.Abs(yScale));
        gameObj.SetActive(false);
    }
}