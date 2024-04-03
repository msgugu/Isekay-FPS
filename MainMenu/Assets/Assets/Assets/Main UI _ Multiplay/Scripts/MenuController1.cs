using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuController1 : MonoBehaviour
{
    public static MenuController1 instance;

    public int NumOfCardToSpawn = 15;
    [SerializeField] private GameObject _canvasObject;
    [SerializeField] private GameObject _cardToSpawn;
    [SerializeField] private Transform _cardParentTrarnsform;

    [HideInInspector] public List<GameObject> Cards = new List<GameObject>();

    public bool IsMenuOpen { get; private set; }

    private bool _cardsHaveSpawned;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        _canvasObject.SetActive(false);
    }

    private void Update()
    {
        if (Keyboard.current.mKey.wasPressedThisFrame)
        {
            ToggleMenu();
        }
    }

    private void ToggleMenu()
    {
        if (IsMenuOpen)
        {
            _canvasObject.SetActive(true);
            IsMenuOpen = true;
        }

        if (!_cardsHaveSpawned)
        {
            SpawnCards();
        }
    }

    private void SpawnCards()
    {
        for (int i = 0; i < NumOfCardToSpawn; i++)
        {
            GameObject card = Instantiate(_cardToSpawn, _cardParentTrarnsform);
            Cards.Add(card);
        }

        _cardsHaveSpawned = true;
    }

}
