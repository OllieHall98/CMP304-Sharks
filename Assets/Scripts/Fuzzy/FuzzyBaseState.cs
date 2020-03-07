using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FLS;
using FLS.Rules;
using FLS.MembershipFunctions;

namespace BoatChase
{
    public abstract class FuzzyBaseState : BaseState
    {

        public FuzzyBaseState(Monster pawn) : base(pawn)
        {
            shark = pawn;
        }

        public override abstract Type Update();
        public override abstract void OnEnter();
    }










}