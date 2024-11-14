using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // HP/MP 系统
    public int maxHP = 100;
    public int currentHP;
    public int maxMP = 50;
    public int currentMP;

    // 背包系统
    public List<Item> inventory = new List<Item>();

    // 保存/加载系统
    private void Awake()
    {
        // 设置单例模式，防止重复的 GameManager
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // 保证在场景切换时不销毁
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 初始化 HP 和 MP
        currentHP = maxHP;
        currentMP = maxMP;
    }

    // HP 操作
    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        if (currentHP <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHP += amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
    }

    public void UseMagic(int amount)
    {
        currentMP -= amount;
        currentMP = Mathf.Clamp(currentMP, 0, maxMP);
    }

    private void Die()
    {
        Debug.Log("Player has died.");
        // 处理玩家死亡的逻辑
    }

    // 背包操作
    public void AddItem(Item item)
    {
        inventory.Add(item);
    }

    public void RemoveItem(Item item)
    {
        inventory.Remove(item);
    }

    public bool HasItem(string itemName)
    {
        return inventory.Exists(item => item.itemName == itemName);
    }

    // 保存/加载系统
    public void SaveGame()
    {
        PlayerPrefs.SetInt("HP", currentHP);
        PlayerPrefs.SetInt("MP", currentMP);
        PlayerPrefs.SetInt("ItemCount", inventory.Count);

        for (int i = 0; i < inventory.Count; i++)
        {
            PlayerPrefs.SetString("Item_" + i, inventory[i].itemName);
            PlayerPrefs.SetInt("ItemQuantity_" + i, inventory[i].quantity);
        }

        PlayerPrefs.Save();
        Debug.Log("Game Saved");
    }

    public void LoadGame()
    {
        currentHP = PlayerPrefs.GetInt("HP", maxHP);
        currentMP = PlayerPrefs.GetInt("MP", maxMP);

        int itemCount = PlayerPrefs.GetInt("ItemCount", 0);
        inventory.Clear();

        for (int i = 0; i < itemCount; i++)
        {
            string itemName = PlayerPrefs.GetString("Item_" + i);
            int quantity = PlayerPrefs.GetInt("ItemQuantity_" + i);

            Item newItem = new Item { itemName = itemName, quantity = quantity };
            AddItem(newItem);
        }

        Debug.Log("Game Loaded");
    }

    [System.Serializable]
    public class Item
    {
        public string itemName;
        public string description;
        public Sprite icon;
        public int quantity;
    }

}

