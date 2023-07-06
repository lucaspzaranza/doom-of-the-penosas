using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputControlsPanel : MonoBehaviour
{
    [SerializeField] private List<GameObject> _panels;

    [SerializeField] private GameObject _closeButton;
    public GameObject CloseButton => _closeButton;

    public void ActivatePanel(string panelName)
    {
        _panels.SingleOrDefault(panel => panel.activeSelf)?.SetActive(false);
        _panels.SingleOrDefault(panel => panel.name.Contains(panelName))?.SetActive(true);
    }
}
