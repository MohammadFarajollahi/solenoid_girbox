
///**********************input_uart**********************
void uart_input() {
  while (mySerial.available() > 0) {
    cmnd = mySerial.readStringUntil('\n');
   
    //lcd_print();
    Serial.println(cmnd);
    uart_text();
  }
}



void uart_text() {
  Serial.println(cmnd);
  data = cmnd.substring(0, 2);
  //*********************motor2********************
  if (data == "td") { //test motor duty
    String ss;
    ss = cmnd.substring(3, 10);
    duty2 = ss.toFloat();
    ss = "test motor duty:" + String(duty2) + "%";
    Serial.println(ss);
    dutyCycleInput2 = map( duty2 , 100 ,0 , 0 , 65535);
    
  }


  if (data == "tf") { //test motor frequency
    String ss;
    ss = cmnd.substring(3, 10);
    freq2 = ss.toFloat();
    ss = "test motor frequency:" + String(freq2) + "Hz";
    Serial.println(ss);
    SetPinFrequencySafe(test_motor, freq2);
  }

}
