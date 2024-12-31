# Stream-Based Messaging System

## Overview
This project is a robust, object-oriented stream-based messaging system implemented in C#. It showcases core principles of OOP such as inheritance, polymorphism, composition, and encapsulation. The system provides functionality for message storage, file-backed persistence, logical partitioning of messages, and subscriber notifications using the Observer pattern.

---

## Features

### 1. Core Components
- **`MsgStream`**
  - Append-only message storage with capacity and operation limits.
  - Ensures no duplicate messages and supports deep copying.

- **`DurableStream`**
  - Extends `MsgStream` to include file-backed persistence.
  - Automatically synchronizes messages with a specified file.

- **`PartitionStream`**
  - Enables partitioning messages by associating them with unique keys.
  - Supports independent operations for each partition.

- **`StreamWithSubscribers`**
  - Adds a subscription mechanism to notify subscribers of new messages.
  - Implements the Observer pattern with flexibility for multiple subscribers.

### 2. Subscribers
- **`ISubscriber` Interface**
  - Abstracts subscriber behavior with a `NewMessage` method.
- **`ExampleSubscriber`**
  - Logs received messages to the console, demonstrating subscription behavior.

### 3. Driver Program
- **`Central`**
  - Demonstrates the functionality of all components through predefined scenarios.
  - Includes testing for partitioning, persistence, deep copying, and subscriber notifications.

---

## Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/your-username/stream-messaging-system.git
