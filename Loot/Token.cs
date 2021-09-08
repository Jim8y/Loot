// Copyright (C) 2021 Jinghui Liao.
// This file belongs to the NEO-GAME-Loot contract developed for neo N3
// Date: Sep-6-2021 
//
// The NEO-GAME-Loot is free smart contract distributed under the MIT software 
// license, see the accompanying file LICENSE in the main directory of
// the project or http://www.opensource.org/licenses/mit-license.php 
// for more details.
// 
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;
using System.Numerics;


namespace Loot
{
    public class Token : Nep11TokenState
    {
        public BigInteger TokenID;

        public BigInteger Credential;

        public static Token MintLoot(UInt160 owner, BigInteger TokenID, BigInteger Credential) => new(owner, TokenID, Credential);

        private Token(UInt160 owner, BigInteger tokenID, BigInteger credential)
        {
            Owner = owner;
            TokenID = tokenID;
            Credential = credential;
            Name = "Loot #" + TokenID;
        }

        public void OwnerOnly()
        {
            Tools.Require(Runtime.CheckWitness(Owner), "Authorization failed.");
        }
    }
}
