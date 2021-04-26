using PF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ETModel
{

    public class CommandResult_Move : ICommandResult
    {
        public List<Vector3> Path; 
        public long unitId;
        public int frame;
        public MoveState state;
        public Vector3 movePosition; 
        public float yRotation;
    }
}
