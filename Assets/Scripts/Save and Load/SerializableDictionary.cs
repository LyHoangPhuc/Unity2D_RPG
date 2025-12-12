using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
//ISerializationCallbackReceiver trong Unity là một giao diện cho phép bạn kiểm soát quá trình tuần tự hóa của các thuộc tính trong một lớp.
//được sử dụng để thực hiện các hành động tùy chỉnh trước (OnAfterDeserialize()) và sau (OnBeforeSerialize()) khi một đối tượng được tuần tự hóa (serialize) hoặc giải tuần tự hóa (deserialize)
//Giúp bạn kiểm soát cách dữ liệu được lưu trữ và phục hồi.
//Hữu ích khi làm việc với các thuộc tính không thể tuần tự hóa tự động hoặc khi cần xử lý thêm dữ liệu.
//có 2 phương thức:
//1.OnAfterDeserialize()
// - Phương thức này được gọi trước khi đối tượng được tuần tự hóa
// - Sử dụng nó để chuẩn bị dữ liệu hoặc thực hiện các thay đổi trước khi quá trình tuần tự hóa diễn ra.
//2.OnBeforeSerialize()
// - Phương thức này được gọi ngay sau khi đối tượng được giải tuần tự hóa
// - Đây là nơi bạn có thể khôi phục trạng thái hoặc thực hiện các hành động cần thiết sau khi dữ liệu đã được tải.
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue> , ISerializationCallbackReceiver
{
    [SerializeField] private List<TKey> keys = new List<TKey>();
    [SerializeField] private List<TValue> values = new List<TValue>();

    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();

        foreach (KeyValuePair<TKey, TValue> pair in this)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }
    public void OnAfterDeserialize() 
    {
        this.Clear();

        if (keys.Count != values.Count)
        {
            Debug.Log("Keys count is not equal to values count");
        }

        for (int i = 0; i < keys.Count; i++)
        {
            this.Add(keys[i], values[i]);
        }
    }

    



}
