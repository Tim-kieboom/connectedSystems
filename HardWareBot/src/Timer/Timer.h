#pragma once
#ifndef TIMER_H
#define TIMER_H

#include <Arduino.h>
#define SET_TIMER_IN_MS true
#define SET_TIMER_IN_US false

class Timer
{
private:
    uint64_t timeNow;
    uint64_t timeBegin;
    bool isMillis;              //if true time in milliSeconds else time is in mircoSeconds

public:
    Timer(bool isMillis);       //also calls startTimer

    void startTimer();          //set timeBegin AND timeNow to the current time
    void resetBeginTime();      //set timeBegin to the current time
    
    bool waitTime(int time); 

private:
    uint64_t getCurrentTime();
    void updateTimer();         //set timeNow to the current time
};

#endif