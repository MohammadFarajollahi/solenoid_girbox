

void timer_() {


  //////////timer//////////////
  static unsigned long timer = millis();
  static int deciSeconds = 0;
  static int deciSeconds1 = 0;

  if (millis() >= timer) {

    deciSeconds1++;
    timer += 100;

    if (deciSeconds1 >= 10) {
      deciSeconds1 = 0;
      ++second;
      //Serial.println(second);
    }
  }
}