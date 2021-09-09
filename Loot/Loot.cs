// Copyright (C) 2021 Jinghui Liao.
// This file belongs to the NEO-GAME-Loot contract developed for neo N3
// Date: Sep-6-2021 
//
// The original contract is deployed on Ethereum 
// The NEO-GAME-Loot is free smart contract distributed under the MIT software 
// license, see the accompanying file LICENSE in the main directory of
// the project or http://www.opensource.org/licenses/mit-license.php 
// for more details.
// 
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Loot
{
    [ManifestExtra("Author", "Jinghui Liao")]
    [ManifestExtra("Email", "jinghui@wayne.edu")]
    [DisplayName("Secure-Loot")]
    [ManifestExtra("Description", "This is a text NFT game developed for neo N3.")]
    [SupportedStandards("NEP-11")]
    [ContractPermission("*", "onNEP11Payment")]
    [ContractTrust("0xd2a4cff31913016155e38e474a2c06d08be276cf")] // GAS contract
    public partial class Loot : Nep11Token<Token>
    {
        public string SourceCode => "https://github.com/Liaojinghui/Loot";
        private static bool Paused() => StateStorage.IsPaused();
        public override string Symbol() => "sLoot";
        public static void OnNEP17Payment(UInt160 from, BigInteger amount, object data) => Tools.Require(!Paused());
        private static readonly StorageMap TokenIndexMap = new(Storage.CurrentContext, (byte)StoragePrefix.Token);
        private static readonly StorageMap TokenMap = new(Storage.CurrentContext, Prefix_Token);
        public static event Action<string> event_msg;

        [Safe]
        public override Map<string, object> Properties(ByteString tokenId)
        {
            Tools.Require(Runtime.EntryScriptHash == Runtime.CallingScriptHash);
            StorageMap tokenMap = new(Storage.CurrentContext, Prefix_Token);
            Token token = (Token)StdLib.Deserialize(tokenMap[tokenId]);
            Map<string, object> map = new();
            map["name"] = token.Name;
            map["owner"] = token.Owner;
            map["tokenID"] = token.TokenID;
            map["credential"] = token.Credential;
            return map;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Token GetToken(BigInteger tokenId)
        {
            Token token = (Token)StdLib.Deserialize(TokenMap[tokenId.ToString()]);
            Tools.Require(token is not null, "Token not exists");
            return token;
        }

        [Safe]
        public BigInteger getCredential(BigInteger tokenId) => GetToken(tokenId).Credential;

        [Safe]
        public string getWeapon(BigInteger credential) => pluck(credential, "WEAPON", weapons);

        [Safe]
        public string getChest(BigInteger credential) => pluck(credential, "CHEST", chestArmor);

        [Safe]
        public string getHead(BigInteger credential) => pluck(credential, "HEAD", headArmor);

        [Safe]
        public string getWaist(BigInteger credential) => pluck(credential, "WAIST", waistArmor);

        [Safe]
        public string getFoot(BigInteger credential) => pluck(credential, "FOOT", footArmor);

        [Safe]
        public string getHand(BigInteger credential) => pluck(credential, "HAND", handArmor);

        [Safe]
        public string getNeck(BigInteger credential) => pluck(credential, "NECK", necklaces);

        [Safe]
        public string getRing(BigInteger credential) => pluck(credential, "RING", rings);

        [Safe]
        private string pluck(BigInteger credential, string keyPrefix, string[] sourceArray)
        {
            var rand = (BigInteger)CryptoLib.Sha256(keyPrefix + credential.ToString());
            string output = sourceArray[(int)rand % sourceArray.Length];
            var greatness = rand % 21;

            if (greatness > 14) output = $"{output} {suffixes[(int)rand % suffixes.Length]}";
            if (greatness >= 19)
            {
                var name_0 = namePrefixes[(int)rand % namePrefixes.Length];
                var name_1 = nameSuffixes[(int)rand % nameSuffixes.Length];
                if (greatness == 19)
                    output = $"\"{name_0} {name_1}\" { output}";
                else
                    output = $"\"{name_0} {name_1}\" { output} +1";
            }
            return output;
        }


        [Safe]
        public string tokenURI(BigInteger tokenId)
        {
            var credential = GetToken(tokenId).Credential;

            //var parts_0 = "<svg xmlns=\"http://www.w3.org/2000/svg\" preserveAspectRatio=\"xMinYMin meet\" viewBox=\"0 0 350 350\"><style>.base { fill: white; font-family: serif; font-size: 14px; }</style><rect width=\"100%\" height=\"100%\" fill=\"black\" /><text x=\"10\" y=\"20\" class=\"base\">";
            //var parts_1 = getWeapon(credential);
            //var parts_2 = "</text><text x=\"10\" y=\"40\" class=\"base\">";
            //var parts_3 = getChest(credential);
            //var parts_4 = "</text><text x=\"10\" y=\"60\" class=\"base\">";
            //var parts_5 = getHead(credential);
            //var parts_6 = "</text><text x=\"10\" y=\"80\" class=\"base\">";
            //var parts_7 = getWaist(credential);
            //var parts_8 = "</text><text x=\"10\" y=\"100\" class=\"base\">";
            //var parts_9 = getFoot(credential);
            //var parts_10 = "</text><text x=\"10\" y=\"120\" class=\"base\">";
            //var parts_11 = getHand(credential);
            //var parts_12 = "</text><text x=\"10\" y=\"140\" class=\"base\">";
            //var parts_13 = getNeck(credential);
            //var parts_14 = "</text><text x=\"10\" y=\"160\" class=\"base\">";
            //var parts_15 = getRing(credential);
            //var parts_16 = "</text></svg>";

            string output = "";//$"{ parts_0} { parts_1} { parts_2} { parts_3} { parts_4} { parts_5} { parts_6} { parts_7} { parts_8}";
            //output = $"{output} { parts_9} { parts_10} { parts_11} { parts_12} { parts_13} { parts_14} { parts_15} { parts_16}";
            //string json = StdLib.Base64Encode($"{{\"name\": \"Bag # {tokenId.ToString()}\", \"description\": \"Loot is randomized adventurer gear generated and stored on chain.Stats, images, and other functionality are intentionally omitted for others to interpret.Feel free to use Loot in any way you want.\", \"image\": \"data:image / svg + xml; base64, { StdLib.Base64Encode(output)} \"}}");
            //output = $"data:application/json;base64, {json}";

            return output;
        }

        /// <summary>
        /// Security Requirements:
        /// 
        /// <0> Has to check the validity of the token Id 
        ///     both the upper and lower bound
        /// 
        /// <1> shall not be called from a contract
        /// 
        /// <3> tx shall fault if token already taken
        /// 
        /// <4> update the token map.
        /// </summary>
        /// <param name="tokenId"></param>
        public void claim(BigInteger tokenId)
        {
            // 222 reserved to the developer
            Tools.Require(!tokenId.IsZero && tokenId < 7778, "Token ID invalid");
            Tools.Require(Runtime.EntryScriptHash == Runtime.CallingScriptHash, "Contract calls are not allowed");
            Transaction tx = (Transaction)Runtime.ScriptContainer;
            MintToken(tokenId, tx.Sender);
            event_msg("Player mints success");
        }

        /// <summary>
        /// Security Requirements:
        /// 
        /// <0> only the owner can call this function
        /// 
        /// <1> the range of the tokenid is to be in (7777, 8001)
        /// </summary>
        /// <param name="tokenId"></param>
        public void ownerClaim(BigInteger tokenId)
        {
            OwnerOnly();
            Tools.Require(tokenId > 7777 && tokenId < 8001, "Token ID invalid");
            var sender = GetOwner();
            MintToken(tokenId, sender);
            event_msg("Owner mints success");
        }

        /// <summary>
        /// Security Requirements:
        /// 
        /// <0> the transaction should `FAULT` if the token already taken
        /// 
        /// <1> has to update the taken map if a new token is mint.
        /// 
        /// </summary>
        /// <param name="tokenId"></param>
        /// <param name="sender"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void MintToken(BigInteger tokenId, UInt160 sender)
        {
            var credential = CheckClaim(tokenId);
            Token token = Token.MintLoot(sender, tokenId, credential);
            Mint(tokenId.ToString(), token);
            TokenIndexMap.Put(tokenId.ToString(), "taken");
        }

        /// <summary>
        /// Security requirements:
        /// 
        /// <0> throw exception if token already taken
        /// 
        /// <1> should get a random number as credential that
        ///     is not predictable and not linked to the tokenId
        /// </summary>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private BigInteger CheckClaim(BigInteger tokenId)
        {
            // <0> -- confirmed
            Tools.Require(TokenIndexMap.Get(tokenId.ToString()) != "taken", "Token already claimed.");
            // <1> -- confirmed
            return Runtime.GetRandom();
        }
    }

    internal enum StoragePrefix
    {
        State = 0x14,
        Owner = 0x15,
        Token = 0x16,
    }
}