//This code inspired by https://github.com/rastikerdar/vazir-font/blob/gh-pages/index.html
function loadDonators(donators) {
  $.getJSON(
    "https://api.github.com/repos/kokabi1365/vajehdan",
    function (repo) {
      $(".section-donation__project-life-span").html(
        toPersianNumber(yearsSince(repo.created_at))
      );
    }
  );

  $(".section-donation__counter").html(toPersianNumber(donators.length));

  $(".section-donation__amount").html(
    toPersianNumber(
      formatNumber(
        donators
          .map((donator) => donator.amount)
          .reduce((sum, amount) => sum + amount)
      )
    )
  );

  for (const donator of donators) {
    donator.photo = donator.photo
      ? donator.photo
      : `https://eu.ui-avatars.com/api/?format=svg&background=random&name=${donator.name}`;

    donator.job = donator.job ? donator.job : "";

    var li = "";

    li = `<li class="donator">`;

    if (donator.website)
      li += `<a href="${donator.website}" rel="nofollow" target="_blank" title="${donator.date}">`;

    li += `<img class="donator__photo" src="${donator.photo}" alt="${donator.name}">
    <p class="donator__name">${donator.name}</p>`;

    if (donator.job) li += `<span class="donator__job">${donator.job}</span>`;

    if (donator.website) li += "</a>";

    li += "</li>\n";

    $(".section-donation__donators").append(li);
  }
}

function yearsSince(date) {
  var seconds = Math.round((new Date() - new Date(date)) / 1000);
  var interval = seconds / 31536000;
  return Math.round(interval);
}

function toPersianNumber(n) {
  const farsiDigits = ["۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹"];
  return n.toString().replace(/\d/g, (x) => farsiDigits[x]);
}

function formatNumber(x) {
  return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, "٬");
}

$(function () {
  $.getJSON("https://github.com/kokabi1365/Vajehdan/blob/master/docs/scripts/donators.json").done(function (donators) {
    donators.sort(function (a, b) {
      return b.date.localeCompare(a.date);
    });
    loadDonators(donators);
  });
});
