using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;    //System.IO Cung cấp các lớp để làm việc với các tệp và luồng (streams).
                    //Các phương thức được sử dụng bên dưới:
                    //Path.Combine để kết hợp nhiều chuỗi thành một đường dẫn hợp lệ cho hệ thống tệp. Tự động xử lý các dấu phân cách đường dẫn(như \ hoặc /)
                    //Path.GetDirectoryName để lấy phần thư mục của một đường dẫn tệp đầy đủ. Nó trả về một chuỗi chứa đường dẫn đến thư mục mà tệp đang nằm trong đó.
                    //Directory.CreateDirectory để tạo một thư mục mới trong hệ thống tệp. Nếu thư mục đã tồn tại, phương thức này sẽ không gây ra lỗi mà chỉ đơn giản là bỏ qua yêu cầu tạo.
                    //Lớp FileStream: để làm việc với các tệp nhị phân. Nó cho phép bạn thực hiện các thao tác đọc và ghi trên tệp
                    //              FileMode.Create: Tạo tệp mới hoặc ghi đè tệp đã tồn tại
                    //              FileMode.Open: Mở tệp đã tồn tại.
                    //              FileMode.Append: Mở tệp và thêm dữ liệu vào cuối tệp.
                    //Lớp StreamWriter: để ghi dữ liệu văn bản vào một luồng. Nó hỗ trợ việc ghi các chuỗi và tự động xử lý việc mã hóa ký tự.
                    //Lớp StreamReader: để đọc dữ liệu văn bản từ một luồng (stream), các phương thức ReadLine, Read, và ReadToEnd

//using để 1.Khai báo không gian tên: cho phép truy cập trực tiếp vào các lớp, phương thức, và thuộc tính trong không gian tên mà không cần phải ghi rõ tên không gian tên mỗi lần (được sử dụng như khi khai báo thư viện bên trên)
//         2.Quản lý tài nguyên: đảm bảo rằng các đối tượng thực hiện IDisposable (có phương thức Dispose) sẽ được giải phóng tài nguyên đúng cách khi không còn sử dụng. (sự dụng trong trường hợp bên dưới)

public class FileDataHandler
{
    private string dataDirPath = "";        //đường dẫn đến thư mục chứa tệp dữ liệu
    private string dataFileName = "";       //tên tệp dữ liệu

    public FileDataHandler(string _dataDirPath, string _dataFileName)
    {
        dataDirPath = _dataDirPath;
        dataFileName = _dataFileName;
        //Debug.Log($"[FileDataHandler] Constructor - DataDir: {dataDirPath}"); // COMMENT: Debug constructor
        //Debug.Log($"[FileDataHandler] Constructor - FileName: {dataFileName}"); // COMMENT: Debug filename
        //Debug.Log($"[FileDataHandler] Full Path: {Path.Combine(dataDirPath, dataFileName)}"); // COMMENT: Debug full path
    }
    public void Save(GameData _data)
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            //JsonUtility.ToJson là một phương thức thuộc về lớp JsonUtility (cung cấp các phương thức để làm việc với dữ liệu JSON), để chuyển đổi đối tượng thành định dạng JSON
            //Chuyển đổi đối tượng GameData thành chuỗi JSON
            string dataToStore = JsonUtility.ToJson(_data, true);

            //using bên dưới để đảm bảo rằng fileStream sẽ được giải phóng tự động khi ra khỏi khối using, giúp tránh rò rỉ tài nguyên.           
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error on trying to save data to file" + fullPath + "\n" + e);
        }
    }
    public GameData Load()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        GameData loadData = null;

        //Kiểm tra xem tệp có tồn tại không trước khi cố gắng mở nó
        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }
                //JsonUtility.FromJson để chuyển đổi chuỗi JSON thành đối tượng 
                loadData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("Error on trying to load data from file:" + fullPath + "\n" + e);
            }
        }
        return loadData;
    }
    public void Delete()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        if(File.Exists(fullPath))
            File.Delete(fullPath);
    }
    
}

