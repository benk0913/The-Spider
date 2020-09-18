using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class RoomsManager : MonoBehaviour, ISaveFileCompatible
{
    public static RoomsManager Instance;

    public List<RoomData> Rooms = new List<RoomData>();

    void Awake()
    {
        Instance = this;
    }

    public void AddCurrentRoom()
    {
        string SceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        if(GetRoom(SceneName) != null)
        {
            return;
        }

        Rooms.Add(new RoomData(SceneName));
    }

    public void AddItem(GameObject itemObject)
    {
        string SceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        RoomData room = GetRoom(SceneName);
        if (room == null)
        {
            return;
        }

        room.Items.Add(new RoomItemData(itemObject.name, itemObject.transform.position));
    }

    public RoomData GetRoom(string roomKey)
    {
        foreach (RoomData room in Rooms)
        {
            if (room.Name == roomKey)
            {
                return room;
            }
        }

        return null;
    }

    public RoomItemData GetItem(string itemKey)
    {
        return GetRoom(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name).GetItem(itemKey);
    }

    public void FromJSON(JSONNode node)
    {
        Rooms.Clear();
        for(int i=0;i<node.Count;i++)
        {
            RoomData room = new RoomData("Unknown");
            room.FromJSON(node[i]);
            Rooms.Add(room);
        }
    }

    public void ImplementIDs()
    {
        throw new System.NotImplementedException();
    }

    public JSONNode ToJSON()
    {
        JSONClass node = new JSONClass();

        for(int i=0;i<Rooms.Count;i++)
        {
            node[i] = Rooms[i].ToJSON();
        }

        return node;
    }
}

public class RoomData : ISaveFileCompatible
{
    public string Name;
    public List<RoomItemData> Items = new List<RoomItemData>();

    public RoomData(string roomKey)
    {
        this.Name = roomKey;
    }

    public RoomItemData GetItem(string itemKey)
    {
        foreach(RoomItemData item in Items)
        {
            if(item.PrefabKey == itemKey)
            {
                return item;
            }
        }

        return null;
    }

    public void FromJSON(JSONNode node)
    {
        this.Name = node["Name"];

        Items.Clear();
        for (int i = 0; i < node["Items"].Count; i++)
        {
            RoomItemData item = new RoomItemData("Unknown", Vector3.one);
            item.FromJSON(node["Items"][i]);
            Items.Add(item);
        }
    }

    public void ImplementIDs()
    {
        throw new System.NotImplementedException();
    }

    public JSONNode ToJSON()
    {
        JSONClass node = new JSONClass();
        node["Name"] = this.Name;

        for(int i=0;i<Items.Count;i++)
        {
            node["Items"][i] = Items[i].ToJSON();
        }

        return node;
    }
}

public class RoomItemData : ISaveFileCompatible
{
    public string PrefabKey;

    public float PositionX;
    public float PositionY;
    public float PositionZ;

    public RoomItemData(string itemKey, Vector3 position)
    {
        PrefabKey = itemKey;
        UpdatePosition(position);
    }

    public void FromJSON(JSONNode node)
    {
        this.PrefabKey = node["PrefabKey"];
        this.PositionX = float.Parse(node["PositionX"], System.Globalization.CultureInfo.InvariantCulture);
        this.PositionY = float.Parse(node["PositionY"], System.Globalization.CultureInfo.InvariantCulture);
        this.PositionZ = float.Parse(node["PositionZ"], System.Globalization.CultureInfo.InvariantCulture);
    }

    public void ImplementIDs()
    {
        throw new System.NotImplementedException();
    }

    public JSONNode ToJSON()
    {
        JSONClass node = new JSONClass();

        node["PrefabKey"] = this.PrefabKey;
        node["PositionX"] = this.PositionX.ToString(System.Globalization.CultureInfo.InvariantCulture);
        node["PositionY"] = this.PositionY.ToString(System.Globalization.CultureInfo.InvariantCulture);
        node["PositionZ"] = this.PositionZ.ToString(System.Globalization.CultureInfo.InvariantCulture);

        return node;
    }

    public void UpdatePosition(Vector3 givenPosition)
    {
        PositionX = givenPosition.x;
        PositionY = givenPosition.y;
        PositionZ = givenPosition.z;
    }
}
