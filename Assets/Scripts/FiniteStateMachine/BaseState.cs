using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BoatChase
{
    public abstract class BaseState
    {
        protected Monster shark;
        
        public BaseState(Monster pawn)
        {
            shark = pawn;
        }

        public abstract Type Update();
        public abstract void OnEnter();
    }
}
