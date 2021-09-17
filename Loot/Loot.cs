// Copyright (C) 2021 Jinghui Liao.
// This file belongs to the NEO-GAME-Loot contract developed for neo N3
// Date: Sep-6-2021 
// TestNet: 0xb78af146bd7aa6870bc5e005bb1134d5d1bfd2dc
// 
// This is not an official Loot contract but a migration from solidity to
// C# to demonstrate how to develop Loot-like NFT on N3. 
// The original contract is deployed on Ethereum: 0xff9c1b15b16263c61d017ee9f65c50e4ae0113d7
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
    public partial class Loot : Nep11Token<TokenState>
    {
        public string SourceCode => "https://github.com/Liaojinghui/Loot";
        public override string Symbol() => "sLoot";
        private static readonly StorageMap TokenIndexMap = new(Storage.CurrentContext, (byte)StoragePrefix.Token);
        private static readonly StorageMap TokenMap = new(Storage.CurrentContext, Prefix_Token);
        public static event Action<string> event_msg;

        [Safe]
        public override Map<string, object> Properties(ByteString tokenId)
        {
            Tools.Require(Runtime.EntryScriptHash == Runtime.CallingScriptHash);
            StorageMap tokenMap = new(Storage.CurrentContext, Prefix_Token);
            TokenState token = (TokenState)StdLib.Deserialize(tokenMap[tokenId]);
            Map<string, object> map = new();
            map["name"] = token.Name;
            map["owner"] = token.Owner;
            map["tokenID"] = token.TokenID;
            map["credential"] = token.Credential;
            return map;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private TokenState GetToken(BigInteger tokenId)
        {
            TokenState token = (TokenState)StdLib.Deserialize(TokenMap[tokenId.ToString()]);
            Tools.Require(token is not null, "Token not exists");
            return token;
        }

        [Safe]
        public BigInteger getCredential(BigInteger tokenId) => GetToken(tokenId).Credential;

        [Safe]
        public string getWeapon(BigInteger credential) => pluck(credential, 0xd7a595, weapons);

        [Safe]
        public string getChest(BigInteger credential) => pluck(credential, 0x5a7e36, chestArmor);

        [Safe]
        public string getHead(BigInteger credential) => pluck(credential, 0x0cdfbb, headArmor);

        [Safe]
        public string getWaist(BigInteger credential) => pluck(credential, 0x7dcd1b, waistArmor);

        [Safe]
        public string getFoot(BigInteger credential) => pluck(credential, 0x92877a, footArmor);

        [Safe]
        public string getHand(BigInteger credential) => pluck(credential, 0x323282, handArmor);

        [Safe]
        public string getNeck(BigInteger credential) => pluck(credential, 0x0d59c2, necklaces);

        [Safe]
        public string getRing(BigInteger credential) => pluck(credential, 0xfae431, rings);

        [Safe]
        private string pluck(BigInteger credential, BigInteger keyPrefix, string[] sourceArray)
        {
            var rand = credential ^ keyPrefix;
            // Here i can not use sourceArray[(int)rand % sourceArray.Length];
            // I have no idea what causes this issue.
            var value = rand % sourceArray.Length;
            string output = sourceArray[(int)rand % sourceArray.Length];
            var greatness = rand % 21;

            value = rand % suffixes.Length;
            if (greatness > 14) output = $"{output} {suffixes[(int)value]}";
            if (greatness >= 19)
            {
                value = rand % namePrefixes.Length;
                var name_0 = namePrefixes[(int)value];
                value = rand % nameSuffixes.Length;
                var name_1 = nameSuffixes[(int)value];
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
            var token = GetToken(tokenId);
            var parts = new string[19];
            parts[0] = "<svg xmlns=\"http://www.w3.org/2000/svg\" preserveAspectRatio=\"xMinYMin meet\" viewBox=\"0 0 350 350\">" +
                "<style>.base { fill: white; font-family: serif; font-size: 14px; }</style>" +
                "<rect width=\"100%\" height=\"100%\" fill=\"black\" />" +
                "<text x=\"10\" y=\"20\" class=\"base\">";
            parts[1] = $"{token.Name}";
            parts[2] = "</text><text x=\"10\" y=\"40\" class=\"base\">";
            parts[3] = getWeapon(token.Credential);
            parts[4] = "</text><text x=\"10\" y=\"60\" class=\"base\">";
            parts[5] = getChest(token.Credential);
            parts[6] = "</text><text x=\"10\" y=\"80\" class=\"base\">";
            parts[7] = getHead(token.Credential);
            parts[8] = "</text><text x=\"10\" y=\"100\" class=\"base\">";
            parts[9] = getWaist(token.Credential);
            parts[10] = "</text><text x=\"10\" y=\"120\" class=\"base\">";
            parts[11] = getFoot(token.Credential);
            parts[12] = "</text><text x=\"10\" y=\"140\" class=\"base\">";
            parts[13] = getHand(token.Credential);
            parts[14] = "</text><text x=\"10\" y=\"160\" class=\"base\">";
            parts[15] = getNeck(token.Credential);
            parts[16] = "</text><text x=\"10\" y=\"180\" class=\"base\">";
            parts[17] = getRing(token.Credential);
            parts[18] = "</text></svg>";

            string output = $"{parts[0]} {parts[1]} {parts[2]} {parts[3]} {parts[4]} {parts[5]} {parts[6]} {parts[7]} {parts[8]}";
            output = $"{output} {parts[9]} {parts[10]} {parts[11]} {parts[12]} {parts[13]} {parts[14]} {parts[15]} {parts[16]} {parts[17]} {parts[18]}";
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
            TokenState token = TokenState.MintLoot(sender, tokenId, credential);
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
        Owner = 0x15,
        Token = 0x16,
    }
}