using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Line : MonoBehaviour
{

    public List<Vector3> poss = new List<Vector3>();

    public GameObject prefab;

    List<GameObject> gos = new List<GameObject>();
    Queue<GameObject> pool = new Queue<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        ShowLine();
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void ShowLine()
    {


        for (int i = 0; i < poss.Count; i++)
        {


            Vector3 curr;
            Vector3 next;
            if (i < poss.Count - 2)
            {
                curr = poss[i];
                next = poss[i + 1];
            }
            else
            {
                curr = poss[i];
                next = poss[i - 1];
            }
            GameObject go = Instantiate(prefab, transform);

            go.transform.position = curr;
            if (curr.x == next.x)
            {
                go.transform.localEulerAngles = new Vector3(90, 90, 0);
            }
            else if (curr.z == next.z)
            {
                go.transform.localEulerAngles = new Vector3(90, 0, 0);
            }


        }


    }

    public class MyStream : Stream
    {
        public override bool CanRead => throw new System.NotImplementedException();

        public override bool CanSeek => throw new System.NotImplementedException();

        public override bool CanWrite => throw new System.NotImplementedException();

        public override long Length => throw new System.NotImplementedException();

        public override long Position { get => position; set => position = value; }



        List<FileStream> fileStreams = new List<FileStream>();

        private long position;


        public MyStream(List<string> paths)
        {
            foreach (var path in paths)
            {
                FileStream fileStream = File.OpenRead(path);
                fileStreams.Add(fileStream);
            }
        }



        public override void Flush()
        {
            throw new System.NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new System.NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new System.NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new System.NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new System.NotImplementedException();
        }

        public class ItemStream { 
        
           // public FileStream fil
        
        }


    }



}
