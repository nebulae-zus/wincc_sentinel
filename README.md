# WinCC Sentinel

## Introducción

Aplicación tipo consola que vigila el Log generado por el APP WinCC y tan pronto detecta el texto 'REQSTATE_CONNECTION_ERROR' agenda un reinicio del APP WinCC.  

La aplicacion wincc_sentinel.exe depende de los siguientes argumentos:  
1. Ruta del Log WinCC.  Ej: ```C:\Program Files\Siemens\Wincc\Diagnose\ModbusTCPIP_Chanel_01.LOG```    
2. Horario permitido para reniciar WinCC. se pueden especificar múltiples horas al día separadas por guiones ('-'), formato HH:mm-HH:mm-HH:mm... Ej: ```08:40-11:00-23:55-00:00```    
3. Ruta del Log generado por esta aplicación (wincc_sentinel). Ej: ```C:\Users\foo\bar\wincc_sentinel_log.txt```  
4. Ruta del script VBS para detener los procesos WinCC.  Ej: ```C:\Program Files\Siemens\WinCC\bin\Reset_winCC.vbs```  
5. Ruta del ejecutable .exe para inicar los procesos WinCC.  Ej: ```C:\Program Files\Siemens\WinCC\bin\AutoStartRT.exe```  
6. Argumentos requeridos por el ejecutable del punto anterior.  Ej: ```D:\ZUs_new\soft\G_M\G_M.MPC /Activ:yes /LANG=ESP /EnableBreak:no```  
  
### Ejemplo de linea de comando para ejecutar wincc_sentinel:
```
> wincc_sentinel.exe "C:\Program Files\Siemens\Wincc\Diagnose\ModbusTCPIP_Chanel_01.LOG" "08:40-11:00-23:55-00:00" "C:\Users\foo\bar\wincc_sentinel_log.txt" "C:\Program Files\Siemens\WinCC\bin\Reset_winCC.vbs" "C:\Program Files\Siemens\WinCC\bin\AutoStartRT.exe" "D:\ZUs_new\soft\G_M\G_M.MPC /Activ:yes /LANG=ESP /EnableBreak:no"  
```

## Requerimientos

Windows XP o superior con .Net Framework 3.5 o superior habilitado

## Despliegue

Descargue este repositorio ya sea por linea de comando ```git clone https://github.com/nebulae-zus/wincc_sentinel.git``` o dando  [click aquí](https://github.com/nebulae-zus/wincc_sentinel/archive/master.zip).  

Dentro del directorio descargado encontrará el binario ejecutable wincc_sentinel.exe en la ruta ```wicc_sentinel/bin/Release```, para la correcta ejecución se requieren todos los archivos del directorio Release

Se requiere que el aplicativo wincc_sentinel.exe sea ejecutado cuando el sistema operativo inicie, para esto puede usarse el Registry de Windows.  En caso de requerir ayuda de como hacerlo, puede referirse a [https://www.akadia.com/services/windows_registry.html](https://www.akadia.com/services/windows_registry.html).  Es necesario ingresar todos los argumentos requeridos tal cual se muestra en la introducción de este documento.

## Notas

Se debe tener presente que en caso de un fallo critico en wincc_sentinel.exe, el programa reportará en el Log lo ocurrido y se dentrá la ejecución del mismo.  El aplicativo no posee medios para correr de nuevo automaticamente.  

Cuando configure la ejecución automatica de wincc_sentinel desde el registry se debe verificar en el Log si el proceso ha iniciado de forma correcta.  

En el Log de wincc_sentinel se registrará los parámetros de inicio, la detección de un error en WinCC y el momento de reincio de WinCC.  El siguiente es un ejemplo de dicho log:

```
2019-01-28_13:18:10: wincc_sentinel started
2019-01-28_13:18:10: program arguments:
2019-01-28_13:18:10:    - logFilePath: C:\Users\xyz\Documents\ModbusTCPIP_Channel_01.LOG
2019-01-28_13:18:10:    - stopProcess: C:\Users\xyz\Documents\Reset_WinCC.vbs
2019-01-28_13:18:10:    - startProcess: C:\Windows\System32\AutoStartRT.exe
2019-01-28_13:18:10:    - startProcessArgs:  C:\Users\xyz\Documents\Reset_WinCC.vbs a b c
2019-01-28_13:18:10:    - timesStr: 13:05-13:07-13:09
2019-01-28_13:19:50: WinCC fault detected: 2018-10-18 10:27:31,140 ERROR    -RQ Terminate flexChan ConnDown: [ZUS_SC] - REQSTATE_CONNECTION_ERROR
2019-01-28_13:23:06: wincc_sentinel started
2019-01-28_13:23:06: program arguments:
2019-01-28_13:23:06:    - logFilePath: C:\Users\xyz\Documents\ModbusTCPIP_Channel_01.LOG
2019-01-28_13:23:06:    - stopProcess: C:\Users\xyz\Documents\Reset_WinCC.vbs
2019-01-28_13:23:06:    - startProcess: C:\Windows\System32\AutoStartRT.exe
2019-01-28_13:23:06:    - startProcessArgs:  C:\Users\xyz\Documents\Reset_WinCC.vbs
2019-01-28_13:23:07:    - timesStr: 13:25-13:08-13:30
2019-01-28_13:25:07: WinCC fault detected: 2018-11-12 02:13:47,015 ERROR    -RQ Terminate flexChan ConnDown: [ZUS_SC2] - REQSTATE_CONNECTION_ERROR
2019-01-28_13:25:07: WinCC restart authorized time detected: 13:25
2019-01-28_13:25:07: WinCC will be restarted
2019-01-28_13:25:07: WinCC Stopping process Running...
2019-01-28_13:25:08: WinCC Stopping process exited
2019-01-28_13:25:08: WinCC start process launched
2019-01-28_13:27:08: WinCC fault detected: 2018-08-17 14:25:43,531 ERROR    -RQ Terminate flexChan ConnDown: [ZUS_SC] - REQSTATE_CONNECTION_ERROR
2019-01-28_13:30:08: WinCC restart authorized time detected: 13:30
2019-01-28_13:30:08: WinCC will be restarted
2019-01-28_13:30:08: WinCC Stopping process Running...
2019-01-28_13:30:09: WinCC Stopping process exited
2019-01-28_13:30:09: WinCC start process launched
```