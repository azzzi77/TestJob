using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private GameObject healthBar;
    [SerializeField] private float healthBarCorrection;
    [SerializeField] private RectTransform targetCanvas;
    [SerializeField] private int Health = 3;
    [SerializeField] private Slider SliderHP;

    private Target _target;
    // Start is called before the first frame update
    void Start()
    {

        _target = FindObjectOfType<Target>();
    }

    public int SetDamage(int d)
    {
        Health--;
        SliderHP.value = Health;

        if (Health <= 0) healthBar.SetActive(false);
        return Health;
    }

    // Update is called once per frame
    void Update()
    {
        if (Health > 0) transform.LookAt(_target.MyPlayer.transform.position);
        RepositionHealthBar();
       
    }
    private void RepositionHealthBar()
    {
        Vector3 monsterPosition = new Vector3(transform.position.x, transform.position.y + healthBarCorrection, transform.position.z);
        healthBar.GetComponent<Transform>().position = Camera.main.WorldToScreenPoint(monsterPosition); 

    }
}
