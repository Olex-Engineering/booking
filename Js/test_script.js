(async () => {
  const requests = [];
  for (let i = 0; i < 6000; i++) {
    requests.push(
      fetch("http://localhost:5014/booking", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          resourceId: "123e4567-e89b-12d3-a456-426614174000",
          userId: "01a0e6b2-f1d9-74c8-b580-d535f7b3b9e3",
          from: "2026-11-28T07:37:24.202Z",
          to: "2026-12-28T06:37:24.202Z",
        }),
      }),
    );
  }

  await Promise.all(requests);

  const allBookings = await fetch("http://localhost:5014/booking/list");
  const allBookingsJson = await allBookings.json();

  console.log(allBookingsJson);
})();
