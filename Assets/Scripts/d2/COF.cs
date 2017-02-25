﻿using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class COF
{
    public struct Layer
    {
        public string dccFilename;
        public string name;
    }

    public struct ImportResult
    {
        public Layer[] layers;
        public int framesPerDirection;
        public int directionCount;
        public int layerCount;
        public byte[] priority;
    }

    static public readonly string[] layerNames = { "HD", "TR", "LG", "RA", "LA", "RH", "LH", "SH", "S1", "S2", "S3", "S4", "S5", "S6", "S7", "S8" };
    static Dictionary<string, ImportResult> cache = new Dictionary<string, ImportResult>();

    static public ImportResult Load(Obj obj)
    {
        string _base = obj._base.Replace('\\', '/');
        string token = obj.token;
        string mode = obj.mode;
        string _class = obj._class;
        
        string cofFilename = "Assets/d2/" + _base + "/" + token + "/cof/" + token + mode + _class + ".cof";
        cofFilename.ToLower();
        if (cache.ContainsKey(cofFilename))
        {
            return cache[cofFilename];
        }

        ImportResult result = new ImportResult();

        byte[] bytes = File.ReadAllBytes(cofFilename);
        var stream = new MemoryStream(bytes);
        var reader = new BinaryReader(stream);

        result.layerCount = reader.ReadByte();
        result.framesPerDirection = reader.ReadByte();
        result.directionCount = reader.ReadByte();
        stream.Seek(25, SeekOrigin.Current);

        result.layers = new Layer[16];

        for (int i = 0; i < result.layerCount; ++i)
        {
            int compositIndex = reader.ReadByte();
            string compositName = layerNames[compositIndex];

            // shadows
            reader.ReadByte();
            reader.ReadByte();

            // transparency
            reader.ReadByte();
            reader.ReadByte();

            string weaponClass = System.Text.Encoding.Default.GetString(reader.ReadBytes(3));
            reader.ReadByte(); // zero byte from zero-terminated weapon class string
            string sptr = obj.layers[compositIndex];
            result.layers[compositIndex].dccFilename = "Assets/d2/" + _base + "/" + token + "/" + compositName + "/" + token + compositName + sptr + mode + weaponClass + ".dcc";
            result.layers[compositIndex].name = compositName + " " + sptr;
        }

        stream.Seek(result.framesPerDirection, SeekOrigin.Current);
        result.priority = reader.ReadBytes(result.directionCount * result.framesPerDirection * result.layerCount);

        AnimData animData = new AnimData();
        if (AnimData.Find(token + mode + _class, ref animData))
        {
            //Debug.Log(cofFilename + " " + framesPerDirection + " anim data found " + animData.framesPerDir + " " + animData.speed);
        }

        cache.Add(cofFilename, result);
        return result;
    }
}