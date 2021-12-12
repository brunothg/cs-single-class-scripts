using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CS_Single_Class_Scripts;
using NUnit.Framework;

namespace Tests
{
    public class JsonTests
    {
        private const string JsonString = @"{
    ""v1"": ""Das ist ein\\\"" string"",
    ""v2"": true,
    ""v3"": 123.394,
    ""v4"": [1,2, 1.5],
    ""v5"": [
        {""v51"": false, ""v52"": 1.2},
        {""v51"": true, ""v52"": 1.3},
        {""v51"": true, ""v52"": 1.4}
    ],
    ""v6"": null
}";

        private static readonly Json.JValue JsonValue = new Json.JObject(new Dictionary<string, Json.JValue>
        {
            ["v1"] = new Json.JString("Das ist ein\\\" string"),
            ["v2"] = new Json.JBoolean(true),
            ["v3"] = new Json.JNumber(123.394),
            ["v4"] = new Json.JArray(new List<Json.JValue>
                {
                    new Json.JNumber(1),
                    new Json.JNumber(2),
                    new Json.JNumber(1.5)
                }
            ),
            ["v5"] = new Json.JArray(new List<Json.JValue>
                {
                    new Json.JObject(new Dictionary<string, Json.JValue>
                    {
                        ["v51"] = new Json.JBoolean(false),
                        ["v52"] = new Json.JNumber(1.2)
                    }),
                    new Json.JObject(new Dictionary<string, Json.JValue>
                    {
                        ["v51"] = new Json.JBoolean(true),
                        ["v52"] = new Json.JNumber(1.3)
                    }),
                    new Json.JObject(new Dictionary<string, Json.JValue>
                    {
                        ["v51"] = new Json.JBoolean(true),
                        ["v52"] = new Json.JNumber(1.4)
                    })
                }
            ),
            ["v6"] = Json.Null
        });

        private static readonly List<string> JsonExamples = new List<string>()
        {
            @"[ 100, 500, 300, 200, 400 ]",
            @"{""color"":""red"",""value"":""#f00""}",
            @"{""id"":""0001"",""type"":""donut"",""name"":""Cake"",""ppu"":0.55,""batters"":{""batter"":[{""id"":""1001"",""type"":""Regular""},{""id"":""1002"",""type"":""Chocolate""},{""id"":""1003"",""type"":""Blueberry""},{""id"":""1004"",""type"":""Devil's Food""}]},""topping"":[{""id"":""5001"",""type"":""None""},{""id"":""5002"",""type"":""Glazed""},{""id"":""5005"",""type"":""Sugar""},{""id"":""5007"",""type"":""Powdered Sugar""},{""id"":""5006"",""type"":""Chocolate with Sprinkles""},{""id"":""5003"",""type"":""Chocolate""},{""id"":""5004"",""type"":""Maple""}]}",
            @"[{""id"":""0001"",""type"":""donut"",""name"":""Cake"",""ppu"":0.55,""batters"":{""batter"":[{""id"":""1001"",""type"":""Regular""},{""id"":""1002"",""type"":""Chocolate""},{""id"":""1003"",""type"":""Blueberry""},{""id"":""1004"",""type"":""Devil's Food""}]},""topping"":[{""id"":""5001"",""type"":""None""},{""id"":""5002"",""type"":""Glazed""},{""id"":""5005"",""type"":""Sugar""},{""id"":""5007"",""type"":""Powdered Sugar""},{""id"":""5006"",""type"":""Chocolate with Sprinkles""},{""id"":""5003"",""type"":""Chocolate""},{""id"":""5004"",""type"":""Maple""}]},{""id"":""0002"",""type"":""donut"",""name"":""Raised"",""ppu"":0.55,""batters"":{""batter"":[{""id"":""1001"",""type"":""Regular""}]},""topping"":[{""id"":""5001"",""type"":""None""},{""id"":""5002"",""type"":""Glazed""},{""id"":""5005"",""type"":""Sugar""},{""id"":""5003"",""type"":""Chocolate""},{""id"":""5004"",""type"":""Maple""}]},{""id"":""0003"",""type"":""donut"",""name"":""Old Fashioned"",""ppu"":0.55,""batters"":{""batter"":[{""id"":""1001"",""type"":""Regular""},{""id"":""1002"",""type"":""Chocolate""}]},""topping"":[{""id"":""5001"",""type"":""None""},{""id"":""5002"",""type"":""Glazed""},{""id"":""5003"",""type"":""Chocolate""},{""id"":""5004"",""type"":""Maple""}]}]",
            @"{""id"":""0001"",""type"":""donut"",""name"":""Cake"",""image"":{""url"":""images/0001.jpg"",""width"":200,""height"":200},""thumbnail"":{""url"":""images/thumbnails/0001.jpg"",""width"":32,""height"":32}}",
        };

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestIndexer()
        {
            Assert.AreEqual(2, JsonValue["v4"][1].Value);
            Assert.AreEqual(true, JsonValue["v5"][2]["v51"].Value);
            Assert.AreEqual(1.4, JsonValue["v5"][2]["v52"].Value);
            Assert.Catch(() =>
            {
                var kp = JsonValue["kp"][2]["v52"].Value;
            });
        }

        [Test]
        public void TestReadJson()
        {
            var json = Json.ReadJson(JsonString, out var ok);
            Console.Out.WriteLine(json.AsString());
            Assert.True(ok);
            Assert.AreEqual(JsonValue, json);
        }

        [Test]
        public void TestWriteJson()
        {
            var json = Json.WriteJson(JsonValue);
            Console.Out.WriteLine(json);
            Assert.AreEqual(
                Regex.Replace(JsonString, @"\s+", ""),
                Regex.Replace(json, @"\s+", "")
            );
        }


        [Test]
        public void TestJsonSamples()
        {
            foreach (var jsonExample in JsonExamples)
            {
                var jValue = Json.ReadJson(jsonExample, out var ok);
                Assert.True(ok);
                var json = Json.WriteJson(jValue);
                Assert.AreEqual(
                    Regex.Replace(jsonExample, @"\s+", ""),
                    Regex.Replace(json, @"\s+", "")
                );
            }
        }
    }
}