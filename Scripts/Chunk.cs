using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Map
{
    [Serializable]
    public class Chunk : MonoBehaviour
    {

        public string type;
        public Vector3Int coord;
        /// <summary>
        /// 损耗值 ： 上 下 左 右
        /// </summary>
        public Vector4 dirLoss;


        /// <summary>
        /// 旋转
        /// </summary>
        /// <param name="dir">1:顺时针 或者 -1:逆时针 </param>
        public void SetRot(int dir = 1)
        {
            transform.localEulerAngles += new Vector3(0, dir * 90, 0);

        }


        public void Init(ChunkData data)
        {
            transform.position = data.pos;
            transform.rotation = data.quaternion;
            dirLoss = data.dirLoss;

            float angleY = transform.localEulerAngles.y;
            if (angleY == 90)
            {
                RotDirLoss(1);
            }
            else if (angleY == -90)
            {
                RotDirLoss(-1);
            }
            else if (angleY == 180 || angleY == -180)
            {
                RotDirLoss(1);
                RotDirLoss(1);
            }

        }


        public ChunkData GetData()
        {
            ChunkData data = new ChunkData();
            data.type = type;
            data.coord = coord;
            data.pos = transform.position;
            data.quaternion = transform.rotation;
            data.dirLoss = dirLoss;

            return data;
        }


        void RotDirLoss(int dir = 1)
        {
            if (dir > 0)
            {
                float t = dirLoss.x;
                dirLoss.x = dirLoss.z;
                dirLoss.y = dirLoss.w;
                dirLoss.z = dirLoss.y;
                dirLoss.w = t;
            }
            else
            {
                float t = dirLoss.x;
                dirLoss.x = dirLoss.w;
                dirLoss.y = dirLoss.z;
                dirLoss.z = t;
                dirLoss.w = dirLoss.y;
            }
        }

    }
}
