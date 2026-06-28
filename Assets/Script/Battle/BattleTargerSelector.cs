using UnityEngine;
using System;
using System.Collections.Generic;

public class BattleTargetSelector : MonoBehaviour
{
    public static BattleTargetSelector Instance;

    private bool isSelecting = false;
    private bool enemyOnly = false;
    private Action<BattleUnit> onSelected;

    [Header("Visual")]
    public GameObject targetArrowPrefab;
    private List<GameObject> arrows = new List<GameObject>();

    private void Awake() => Instance = this;

    public void StartSelection(bool selectEnemyOnly, Action<BattleUnit> callback)
    {
        isSelecting = true;
        enemyOnly = selectEnemyOnly;
        onSelected = callback;
        ShowTargetArrows();
    }

    void ShowTargetArrows()
    {
        ClearArrows();
        if (targetArrowPrefab == null) return;

        foreach (var unit in BattleManager.Instance.GetAllUnits())
        {
            if (unit.isEnemy && !unit.isDead)
            {
                GameObject arrow = Instantiate(targetArrowPrefab,
                    unit.gameObject.transform.position + Vector3.up * 0.8f,
                    Quaternion.identity);
                arrows.Add(arrow);
            }
        }
    }

    void ClearArrows()
    {
        foreach (var a in arrows)
            if (a != null) Destroy(a);
        arrows.Clear();
    }

    private void Update()
    {
        if (!isSelecting) return;
        if (!Input.GetMouseButtonDown(0)) return;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Debug.Log($"Klik di posisi: {mousePos}");

        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
        Debug.Log($"Hit: {(hit.collider != null ? hit.collider.gameObject.name : "null")}");

        if (hit.collider != null)
        {
            Enemy enemy = hit.collider.GetComponentInParent<Enemy>();
            Character character = hit.collider.GetComponentInParent<Character>();
            Debug.Log($"enemyOnly: {enemyOnly} | enemy: {enemy} | character: {character}");

            if (enemyOnly && enemy != null)
            {
                Debug.Log("Masuk kondisi enemy!");
                BattleUnit unit = FindUnit(enemy);
                Debug.Log($"Unit found: {unit?.unitName}");
                if (unit != null && !unit.isDead)
                {
                    isSelecting = false;
                    ClearArrows();
                    onSelected?.Invoke(unit);
                }
            }
        }
    }

    BattleUnit FindUnit(Enemy enemy)
    {
        Debug.Log($"Mencari unit, total allUnits: {BattleManager.Instance.GetAllUnits().Count}");
        foreach (var unit in BattleManager.Instance.GetAllUnits())
        {
            Debug.Log($"Unit: {unit.unitName} | enemyRef: {unit.enemyRef}");
            if (unit.enemyRef == enemy) return unit;
        }
        Debug.Log("Unit tidak ditemukan!");
        return null;
    }

    BattleUnit FindUnit(Character character)
    {
        foreach (var unit in BattleManager.Instance.GetAllUnits())
            if (unit.characterRef == character) return unit;
        return null;
    }
}