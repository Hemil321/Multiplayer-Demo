using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private string channelName = "";

    private string token = "";

    public string GetChannelName() { return channelName; }

    public void SetChannelName(string channelName) { 
        this.channelName = channelName; }


    public string GetToken() { return token; }

    public void SetToken(string token) {  this.token = token; }
}
