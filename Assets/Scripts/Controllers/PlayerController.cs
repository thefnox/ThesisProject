using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public GameObject damageIndicator;
    public PlayerState state;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (state == null) return;
        if (state.IsAttacking() && (state.IsInStartup() || state.IsInActive()))
        {
            var move = state.GetMove(state.frame.animState);
            damageIndicator.SetActive(true);
            damageIndicator.transform.localPosition = new Vector3(move.bounds.position.x / 100f, move.bounds.position.y / 100f, move.bounds.position.z / 100f);
            damageIndicator.transform.localScale = new Vector3(move.bounds.size.x / 100f, move.bounds.size.y / 100f, 1f);
        } else
        {
            damageIndicator.SetActive(false);
        }
        damageIndicator.SetActive(state.IsAttacking() && state.IsInActive());
    }
}
