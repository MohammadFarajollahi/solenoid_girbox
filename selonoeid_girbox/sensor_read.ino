float pressure_sensor1;
float pressure_sensor2;
int sensor_read22;
int sensor_read11;

float sensor_read1, sensor_read2;
//***********sensor_read***********
void sensor_read() {
  if (sensor1_send == 1) {
    sensor_read11 = 0;
    sensor_read22 = 0;
    for (int i = 1; i <= 40; i++) {
      sensor_read11 += analogRead(sensor1);
      sensor_read22 += analogRead(sensor2);
      delay(10);
    }

    sensor_read1 = sensor_read11 / 40;
    sensor_read2 = sensor_read22 / 40;

    pressure_sensor1 = sensor_read1 / 48.5;  //36
    pressure_sensor2 = sensor_read2 / 48.5;
    if (pressure_sensor1 < 0) pressure_sensor1 = 0;
    if (pressure_sensor2 < 0) pressure_sensor2 = 0;

    //data_send = "sensor1:" + String(pressure_sensor1) + ":" + "37.5" + ":" + "12v" + "B"  + "sensor2: " + String(pressure_sensor1) + ":" + "37.5" + ":" + "12v" + "B";
    data_send = "sensor1:" + String(pressure_sensor1) + ":" + "37.5" + ":" + "12v" + "B" + "sensor2:" + String(pressure_sensor2) + ":" + "38.5" + ":" + "15v" + "B / C";
    //data_send = "sensor1:" + String(random(0, 300)) + "B";
    Serial.println(data_send);
    //delay(500);

    //Serial.println("s1,34,129,87Bs2,24,19,97Bs3,24,19,97Bs4,24,19,97B");
  }
}
