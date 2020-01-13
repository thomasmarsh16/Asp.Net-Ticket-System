# System Requirements for Asp.Net-Ticket-System 

--- Contents ---

1.0 Introduction

            1.1 Purpose

            1.2 Intended Audience/ Use

            1.3 Scope

            1.4 Definitions and Acronyms

2.0 Overall Description

            2.1 User Needs

            2.2 Assumptions and Dependencies

3.0 System Features and Requirements

            3.1 Functional Requirements

            3.2 System Features

            3.3 Nonfunctional Requirements



# Introduction

1.1 Purpose
           
The purpose of this software project is to construct a ticket system for tasks that can be shared and completed between multiple people while also giving management the ability to monitor workers current tasks and completion rates.

1.2 Intended Audience/Use

Intended audiences for this document are those who would like to see the reasoning behind how this software project has been implemented. This document can be used to see why some design decision were made and the goals that the project worked toward.

1.3 Scope

The main objective for this software project is to provide a ticket system which can enable monitoring and control of task completion for projects. Project managers will be able to view tasks by project groupings, who is working on certain tasks, and how long tasks have been "checked out". Workers meanwhile will be able to view what tasks they have been assigned, who else is working on the tasks, and other data for tasks.

1.4 Definitions and Acronyms

Risks for this project are those involving schedule conflicts caused by workers being assigned more tasks than they can handle. Another risk for this project is that of server and connection issues causing workers to not be able to use the software.


# Overall Description

This software project is meant to be standalone with there being no prior product relations.

2.1 User Needs

Primary users for this project are workers and managers for projects, their needs are viewing and collaborating with others for tasks, controlling task completion, and viewing task / project characteristics pertaining to project success.

Secondary users are those who interact with workers on the project, their needs are for tickets to provide accurate information on project tasks for the workers so that they are completed on time and accurately, enabling secondary users to do their jobs. 

2.2 Assumptions and Dependencies

Factors that can affect fulfillment of user needs and requirements are those involving hosting the software on a web server using Microsoft Azure. These can involve increases in hosting costs which make it too costly to use the service any longer. Another factor for this project is that of maintaining internet connections with users and servers on Azure staying up without downtime.

From the stated factors this project has dependencies involving Azure servers, pricing for hosting remaining within tolerable ranges, and maintaining a connection to users.

# System Features and Requirements

3.1 Functional Requirements

Functional requirements for this project is that it can provide users with task information for projects and group/sort them by user assignment, what projects they are for, when tasks should be completed, and how long a task should be checked out. The software should also enable managers to view worker task completion data, progress on projects, assign and edit tasks, and to better control project completion.

3.2 System Features

System features for this project are that there is an internet connection, hosting of files is done by Microsoft Azure, and the database used is SQL based.

3.3 Nonfunctional Requirements

A nonfunctional requirement for this project is that security is to a high enough level where only authorized users can gain access to task functions and data along with prevention of database attacks such as SQL injections. Performance requirements for this project are that task data is returned to users within a second if there is a steady internet connection. Data transmitted should also be minimized in size to allow faster transfers on low bandwidth connections.

