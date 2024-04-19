#pragma once
#ifndef __MYWIFI_H__
#define __MYWIFI_H__

#include <WiFi.h>
// #include <SPIFFS.h>
#include <Arduino.h>
// #include <AsyncTCP.h>
#include <HTTPClient.h>
#include <ArduinoJson.h>
// #include <ESPAsyncWebServer.h>

struct Position
{
    Position(uint8_t x, uint8_t y)
    {
        this->x = x;
        this->y = y;
    }

    uint8_t x, y;
};

enum Direction
{
    right   = 0,
    up      = 1,
    left    = 2,
    down    = 3,
    idle    = 4
};

//start server
void wifiInit();

Direction getDirection();
void sendStopBot();

#endif