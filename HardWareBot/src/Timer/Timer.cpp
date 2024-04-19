#include "Timer.h"

Timer::Timer(bool isMillis)
: isMillis(isMillis)
{
    startTimer();
}

void Timer::startTimer()
{
    timeBegin = getCurrentTime();
    timeNow   = getCurrentTime();
}

void Timer::updateTimer()
{
    timeNow = getCurrentTime();
}

bool Timer::waitTime(int time)
{
    updateTimer();
    
    if(timeNow - timeBegin > time)
    {
        resetBeginTime();
        return true;
    }

    return false;
}

void Timer::resetBeginTime()
{
    timeBegin = getCurrentTime();
}

uint64_t Timer::getCurrentTime()
{
    if(isMillis)
        return millis();
    
    return micros();
}