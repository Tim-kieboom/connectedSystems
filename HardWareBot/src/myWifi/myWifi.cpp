#include "myWifi.h"

static const uint8_t myBotID = 0;
static Direction direction = idle;

static const char* password = "SlimmeDingenInHuis";//= "fsL6HgjN";
static const char* ssid     = "Kibi-iot";//= "tesla iot";

static const String backendServerURL = "192.168.3.79";
static const int port = 32769;
static const char* path =  "/api/getBotPositions";

void sendStopBot()
{
  if(!WiFi.isConnected())
    return;

  HTTPClient client;

  client.begin(backendServerURL + "/api/sendStopBot");
  //client.addHeader("contet-Type", "application/json");
  client.POST("{botID: "+String(myBotID)+"}");
}

void wifiInit()
{
  // if(!SPIFFS.begin(true))
  // {
  //   Serial.println("SPIFFS coundn't start");
  //   exit(1);
  // }

  WiFi.begin(ssid, password);

  while (WiFi.status() != WL_CONNECTED) 
  {
    delay(1000);
    Serial.println("Connecting...");
  }
  // Serial.print("ip address: ");
  // Serial.println(IP);

  // serverController();

  // server.begin();
}

Direction getDirection()
{
  static Direction previousDirection = idle;

  if(WiFi.status() != WL_CONNECTED)
  {
    return previousDirection;
  }
  //String path = backendServerURL + "/api/getBotPositions";

  WiFiClient wificlient;
  HTTPClient client;

  client.begin(wificlient, backendServerURL, port, path);
  client.addHeader("Content-Type", "application/json");
  int status = client.GET();

  String payload = "";

  if(status > 0)
  {
    payload = client.getString();
    //Serial.println(payload);
  }
  client.end();

  JsonDocument json;

  DeserializationError isJsonError = deserializeJson(json, payload);
  if(isJsonError)
  {
    Serial.println("error decoding json");
    return previousDirection;
  }

  JsonArray currentPositions = json["currentPosition"];

  int size = currentPositions.size();
  for(int i = 0; i < size; i++)
  {
    int botID = currentPositions[i]["botID"];
    Serial.println(String(botID));
    if(botID == myBotID) 
    {
      Direction direction = currentPositions[i]["direction"];
      Serial.println(String(direction));
      return direction;
    }
  }

  return previousDirection;
}

