#include <OneMsTaskTimer.h>
int sensitivity = 200;
int inval = 6;
int outval = 2;
int inIndicator = 14;
int outIndicator = 10;
OneMsTaskTimer_t timer = {5, sendreceive, 0, 0};

volatile bool readingByte = false;
volatile bool sendingByte = false;
volatile byte currentReadBit = 0;
volatile byte currentSendBit = 0;
volatile byte readByte = 0;
volatile byte sendByte = 0;
volatile bool flip = false;

void setup()
{
  OneMsTaskTimer::add(&timer);
  OneMsTaskTimer::start();
  Serial.begin(9600);
  pinMode(inIndicator, OUTPUT);
  pinMode(outIndicator, OUTPUT);
  pinMode(outval, OUTPUT);
  digitalWrite(inIndicator, LOW);
  digitalWrite(outIndicator, LOW);
  digitalWrite(outval, LOW);
}

void loop()
{

}


void sendreceive()
{  
  bool sendBit;
  if (sendingByte)
  {
    //sendBit = (sendByte>>currentSendBit) & 1;
    sendBit = bitRead(sendByte,currentSendBit);
    digitalWrite(outval, sendBit);
    digitalWrite(outIndicator, sendBit);
    ++currentSendBit;
    if (currentSendBit>7)
    {
      sendingByte = false;
      currentSendBit = 0;
      sendingByte = 0;
      sendByte = 0;
    }
  }
  else
  {
    sendByte = 0;
    if (Serial.available() > 0)
    {
      sendByte = Serial.read();
      
      //Serial.print("<S:");
       //Serial.write(sendByte);
      // Serial.print(">");
      sendingByte = true;
      currentSendBit = 0;
      
      //notification bit
      digitalWrite(outval, true);
      digitalWrite(outIndicator, true);
    }
    else
    {
      digitalWrite(outIndicator, LOW);
      digitalWrite(outval, LOW);
    }
  }
  
  
  bool readHigh = analogRead(inval) > sensitivity;
  digitalWrite(inIndicator,readHigh);
  if (readingByte)
  {
     bitWrite(readByte,currentReadBit,readHigh);
     //readByte |= (readHigh << currentReadBit);
     ++currentReadBit;
     if (currentReadBit > 7)
     {
       readingByte = false;
       currentReadBit = 0;
       //send the byte
       //Serial.print("<R:");
       Serial.write(readByte);
       //Serial.print(">");
       //reset the byte
       readByte = 0;
     } 
  }
  else if (readHigh)
  {
    readingByte = true;
  }

  
}










