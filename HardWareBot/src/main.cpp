#include <Arduino.h>
#include "myWifi/myWifi.h"
#include "Timer/Timer.h"

#define EM_BUTTON 14

enum LedPins
{
  upLed = 15,
  downLed = 2,
  leftLed = 0,
  rightLed = 4
};

void doDirection(Direction direction)
{
  digitalWrite(upLed, LOW);
  digitalWrite(downLed, LOW);
  digitalWrite(leftLed, LOW);
  digitalWrite(rightLed, LOW);


  switch(direction)
  {
    case up:
      digitalWrite(upLed, HIGH);
      Serial.println("up");
      break;

    case down:
      digitalWrite(downLed, HIGH);
      Serial.println("down");
      break;

    case left:
     digitalWrite(leftLed, HIGH);
      Serial.println("left");
      break;

    case right:
      digitalWrite(rightLed, HIGH);
      Serial.println("right");
      break;

    case idle:
      Serial.println("idle");
      break;
  }
}

void setup() 
{
  Serial.begin(115200);

  pinMode(upLed,    OUTPUT);
  pinMode(downLed,  OUTPUT);
  pinMode(leftLed,  OUTPUT);
  pinMode(rightLed, OUTPUT);

  pinMode(EM_BUTTON, INPUT);

  wifiInit();
}

void loop() 
{
  static Timer* timer = new Timer(SET_TIMER_IN_MS);

  if(timer->waitTime(200))
  {
    Direction direction = getDirection();
    doDirection(direction);
  }

  // if(digitalRead(EM_BUTTON) == HIGH)
  //   sendStopBot();
}