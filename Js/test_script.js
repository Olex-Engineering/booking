const BASE_URL = "http://localhost:5014/api";
const REQUESTS = 6000;

(async () => {
  // Уникальный период на каждый прогон, чтобы скрипт можно было запускать
  // повторно без перезапуска сервера: иначе второй прогон получит 0 × 201.
  const from = new Date(Date.now() + (365 + Math.random() * 3650) * 86_400_000);
  const to = new Date(from.getTime() + 60 * 60 * 1000);

  const requests = [];
  for (let i = 0; i < REQUESTS; i++) {
    requests.push(
      fetch(`${BASE_URL}/bookings`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          resourceId: "123e4567-e89b-12d3-a456-426614174000",
          userId: "01a0e6b2-f1d9-74c8-b580-d535f7b3b9e3",
          from: from.toISOString(),
          to: to.toISOString(),
        }),
      })
        .then((res) => res.status)
        .catch((err) => `network error: ${err.cause?.code ?? err.message}`),
    );
  }

  const statuses = await Promise.all(requests);

  const counts = {};
  for (const status of statuses) {
    counts[status] = (counts[status] ?? 0) + 1;
  }

  const created = counts[201] ?? 0;
  const conflicts = counts[409] ?? 0;
  const other = REQUESTS - created - conflicts;

  console.log("Статусы:", counts);

  if (created === 1 && conflicts === REQUESTS - 1 && other === 0) {
    console.log(`OK: 1 × 201, ${REQUESTS - 1} × 409, прочих 0`);
  } else {
    console.error(
      `FAIL: ожидали 1 × 201 и ${REQUESTS - 1} × 409, получили ${created} × 201, ${conflicts} × 409, прочих ${other}`,
    );
    process.exitCode = 1;
  }
})();
