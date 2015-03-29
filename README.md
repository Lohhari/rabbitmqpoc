# rabbitmqpoc
RabbitMQPOC - A training exercise / proof of concept for Rabbit message queue usage

This parallelization pipeline demo gets a data file as input, splits it
into many smaller files, processes each small file using parallel
processes and finally combines the subresults into one final result.

The solution is a collection of five executables that together form the
pipeline for processing data. See flowchart.png for the overall process.
There is a message queue between every step. RabbitMQ is used as the
message queue system.

The data represents values in 3D space. For simplicity each cube has
exactly one integer value. Each cube is processed separately. The Process
step doubles each value and the Combine step sums all the results together.

The idea is that three of the steps, Split, Process and Combine can be
heavy and can be parallelized. PushWork is solely a producer and will exit
after completing its work. Collect is a very light step and you must only
run one Collect.exe at a time. The five steps are described in more detail
below.

PushWork
--------
PushWork.exe &lt;datafile.txt&gt;

Enqueue work to the pipeline. The data file format is described below.
You can find a couple of test data files in the testdata folder.

Split
-----
This step will read the data file and split it into X * Y * Z files.
For a 2 x 2 x 2 space you get 8 files.

Process
-------
Read an input file, double the integer value found in it and write it in
an output file. Sleep is used to simulate processing time.

Collect
-------
Wait for all the Process steps to finish. When they have, tell Combine to
start.

Combine
-------
Read all the output files and calculate a sum of all the values.

Data format
-----------
The data format is very simple. See the files in the testdata folder,
they are nearly self-explanatory. The first line tells the dimensions of
the space in X, Y and Z dimensions. The remaining lines are the data. One
integer per cube.
