# MD5 Hash

## Existing Tests

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| MD5-001 | x | Пустая строка даёт стандартный известный MD5 digest | [`HashesTests.cs`](../../../../Tests/DoubleGeometry/Toolkit/Hashes/HashesTests.cs) |
| MD5-002 | x | ASCII-строка `"abc"` даёт стандартный известный MD5 digest | [`HashesTests.cs`](../../../../Tests/DoubleGeometry/Toolkit/Hashes/HashesTests.cs) |
| MD5-003 | x | Не-ASCII строка хешируется через UTF-8 | [`HashesTests.cs`](../../../../Tests/DoubleGeometry/Toolkit/Hashes/HashesTests.cs) |

## Gaps

Пока новых обязательных сценариев сверх этого прямого текущего контракта не выявлено.
